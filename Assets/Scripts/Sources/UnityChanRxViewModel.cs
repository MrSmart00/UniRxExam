﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;

namespace UnityChan.Rx
{
    public struct UnityChanRxViewModelContext
    {
        public float forwardSpeed;
        public float backwardSpeed;
        public float rotateSpeed;
        public float jumpPower;
        public Transform transform;

        public UnityChanRxViewModelContext(float forwardSpeed, float backwardSpeed, float rotateSpeed, float jumpPower, Transform transform)
        {
            this.forwardSpeed = forwardSpeed;
            this.backwardSpeed = backwardSpeed;
            this.rotateSpeed = rotateSpeed;
            this.jumpPower = jumpPower;
            this.transform = transform;
        }
    }

    public class UnityChanRxViewModel : IUnityChanViewModel<UnityChanAnimatorState, UnityChanRxViewModelContext>
    {
        [Inject]
        ICharacterMover mover;

        [Inject]
        IAnimatorStatus<UnityChanAnimatorState> status;

        [Inject]
        IDeltaTime deltaTime;

        private UnityChanLocomotionModel model;

        public void inject(UnityChanRxViewModelContext context)
        {
            model = new UnityChanLocomotionModel(
                forwardSpeed: context.forwardSpeed,
                backwardSpeed: context.backwardSpeed,
                rotateSpeed: context.rotateSpeed,
                jumpPower: context.jumpPower,
                transform: context.transform,
                deltaTime: deltaTime.delta
                );
        }

        public UnityChanViweModelOutput<UnityChanAnimatorState> transform(UnityChanViewModelInput input)
        {
            var update = input.update.Share();
            var infoWOTransition = Observable
                .ZipLatest(update, input.stateInfo, (_update, _info) => _info)
                .Where(_info => !_info.Animator.IsInTransition(0))
                .Share();

            var state = infoWOTransition
                .Do(_info => status.update(_info.StateInfo.fullPathHash))
                .Select(_info => status.currentState)
                .Share();

            var move = state
                .Select(_ => model.convertVelocity(
                    horizontal: mover.GetHorizontalAxis(),
                    vertical: mover.GetVerticalAxis(),
                    isJump: _ == UnityChanAnimatorState.Locomotion && mover.GetJump()));

            var userInput = state
                .Select(_ => (
                speed: mover.GetVerticalAxis(),
                direction: mover.GetHorizontalAxis(),
                isJump: _ == UnityChanAnimatorState.Locomotion && mover.GetJump(),
                isRest: _ == UnityChanAnimatorState.Idle && mover.GetJump()
                ));

            return new UnityChanViweModelOutput<UnityChanAnimatorState>(state: state, userInput: userInput, move: move);
        }
    }
}
