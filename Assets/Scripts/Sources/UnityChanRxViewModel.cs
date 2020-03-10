using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Diagnostics;
using Zenject;

namespace UnityChan.Rx
{
    public struct UnityChanRxViewModelContext
    {
        public float forwardSpeed;
        public float backwardSpeed;
        public float rotateSpeed;
        public Transform transform;

        public UnityChanRxViewModelContext(float forwardSpeed, float backwardSpeed, float rotateSpeed, Transform transform)
        {
            this.forwardSpeed = forwardSpeed;
            this.backwardSpeed = backwardSpeed;
            this.rotateSpeed = rotateSpeed;
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
                transform: context.transform,
                deltaTime: deltaTime.delta
                );
        }

        public UnityChanViweModelOutput<UnityChanAnimatorState> transform(UnityChanViewModelInput input)
        {
            var infoWOTransition = Observable
                .ZipLatest(input.update, input.stateInfo, (_update, _info) => _info)
                .Where(_info => !_info.Animator.IsInTransition(0))
                .Share();

            var state = infoWOTransition
                .Do(_info => status.update(_info.StateInfo.fullPathHash))
                .Select(_info => status.currentState)
                .DistinctUntilChanged()
                .Share();

            var move = infoWOTransition
                .Select(_ => model.convertVelocity(horizontal: mover.HorizontalAxis(), vertical: mover.VerticalAxis())).Debug();

            return new UnityChanViweModelOutput<UnityChanAnimatorState>(state: state, move: move);
        }
    }
}
