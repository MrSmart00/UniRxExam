using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using static UniRx.Triggers.ObservableStateMachineTrigger;
using Zenject;

namespace UnityChan.Rx
{
    public struct ViewModelContext
    {
        public float forwardSpeed;
        public float backwardSpeed;
        public float rotateSpeed;
        public float jumpPower;
        public Transform transform;
        public bool useCurves;
        public float useCurvesHeight;
        public float colliderHight;
        public float colliderCenterY;

        public ViewModelContext(float forwardSpeed, float backwardSpeed, float rotateSpeed, float jumpPower, Transform transform, bool useCurves, float useCurvesHeight, float colliderHight, float colliderCenterY)
        {
            this.forwardSpeed = forwardSpeed;
            this.backwardSpeed = backwardSpeed;
            this.rotateSpeed = rotateSpeed;
            this.jumpPower = jumpPower;
            this.transform = transform;
            this.useCurves = useCurves;
            this.useCurvesHeight = useCurvesHeight;
            this.colliderHight = colliderHight;
            this.colliderCenterY = colliderCenterY;
        }
    }

    public struct ViewModelInput
    {
        public IObservable<Unit> update;
        public IObservable<OnStateInfo> stateInfo;
        public IObservable<float> jumpHeight;
        public IObservable<float> gravityControl;

        public ViewModelInput(IObservable<Unit> update, IObservable<OnStateInfo> stateInfo, IObservable<float> jumpHeight, IObservable<float> gravityControl)
        {
            this.update = update;
            this.stateInfo = stateInfo;
            this.jumpHeight = jumpHeight;
            this.gravityControl = gravityControl;
        }
    }

    public struct ViweModelOutput<State> where State : Enum
    {
        public IObservable<State> state;
        public IObservable<(float speed, float direction, bool isJump, bool isRest)> userInput;
        public IObservable<(Vector3 position, Vector3 rotate, Vector3 jump)> move;
        public IObservable<bool> needsGravity;
        public IObservable<(float centerY, float height)> colliderInfo;

        public ViweModelOutput(IObservable<State> state, IObservable<(float speed, float direction, bool isJump, bool isRest)> userInput, IObservable<(Vector3 position, Vector3 rotate, Vector3 jump)> move, IObservable<bool> needsGravity, IObservable<(float centerY, float height)> colliderInfo)
        {
            this.state = state;
            this.userInput = userInput;
            this.move = move;
            this.needsGravity = needsGravity;
            this.colliderInfo = colliderInfo;
        }
    }

    public class UnityChanRxViewModel : IViewModel<ViewModelContext, ViewModelInput, ViweModelOutput<UnityChanAnimatorState>>
    {
        [Inject]
        ICharacterMover mover;

        [Inject]
        IAnimatorStatus<UnityChanAnimatorState> status;

        [Inject]
        IDeltaTime deltaTime;

        private UnityChanLocomotionModel model;

        public void inject(ViewModelContext context)
        {
            model = new UnityChanLocomotionModel(
                forwardSpeed: context.forwardSpeed,
                backwardSpeed: context.backwardSpeed,
                rotateSpeed: context.rotateSpeed,
                jumpPower: context.jumpPower,
                jumpThreshold: context.useCurvesHeight,
                colliderOffsetY: context.colliderCenterY,
                colliderDefaultHeight: context.colliderHight,
                transform: context.transform,
                deltaTime: deltaTime.delta
                );
        }

        public ViweModelOutput<UnityChanAnimatorState> transform(ViewModelInput input)
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

            var jump = state
                .Where(_ => _ == UnityChanAnimatorState.Jump)
                .Share();

            var needsGravity = Observable
                .CombineLatest(jump, input.gravityControl, (_jump, gravity) => gravity)
                .Select(_ => model.needsGravity(_));

            var colliderInfo = Observable
                .CombineLatest(jump, input.jumpHeight, (_jump, height) => height)
                .Select(_ => model.calculateCurveDelta(_));

            var userInput = state
                .Select(_ => (
                speed: mover.GetVerticalAxis(),
                direction: mover.GetHorizontalAxis(),
                isJump: _ == UnityChanAnimatorState.Locomotion && mover.GetJump(),
                isRest: _ == UnityChanAnimatorState.Idle && mover.GetJump()
                ));

            return new ViweModelOutput<UnityChanAnimatorState>(
                state: state,
                userInput: userInput,
                move: move,
                needsGravity: needsGravity,
                colliderInfo: colliderInfo
                );
        }
    }
}
