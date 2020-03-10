using UnityEngine;

namespace UnityChan.Rx
{
    public enum UnityChanAnimatorState
    {
        Idle,
        Locomotion,
        Jump,
        Rest
    }

    public class UnityChanAnimatorStatus : IAnimatorStatus<UnityChanAnimatorState>
    {
        static int idleState = Animator.StringToHash("Base Layer.Idle");
        static int locoState = Animator.StringToHash("Base Layer.Locomotion");
        static int jumpState = Animator.StringToHash("Base Layer.Jump");
        static int restState = Animator.StringToHash("Base Layer.Rest");

        public UnityChanAnimatorState currentState { get; private set; } = UnityChanAnimatorState.Idle;

        public void update(int stateHash)
        {
            if (stateHash == idleState)
            {
                currentState = UnityChanAnimatorState.Idle;
            }
            else if (stateHash == locoState)
            {
                currentState = UnityChanAnimatorState.Locomotion;
            }
            else if (stateHash == jumpState)
            {
                currentState = UnityChanAnimatorState.Jump;
            }
            else if (stateHash == restState)
            {
                currentState = UnityChanAnimatorState.Rest;
            }
        }
    }
}