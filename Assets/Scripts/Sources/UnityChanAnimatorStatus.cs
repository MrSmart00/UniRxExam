using UnityEngine;

namespace UnityChan.Rx
{
    enum UnityChanAnimatorState
    {
        Idle,
        Locomotion,
        Jump,
        Rest
    }

    struct UnityChanAnimatorStatus : IAnimatorStatus<UnityChanAnimatorState>
    {
        UnityChanAnimatorState state;

        static int idleState = Animator.StringToHash("Base Layer.Idle");
        static int locoState = Animator.StringToHash("Base Layer.Locomotion");
        static int jumpState = Animator.StringToHash("Base Layer.Jump");
        static int restState = Animator.StringToHash("Base Layer.Rest");

        public UnityChanAnimatorState CurrentState()
        {
            return state;
        }

        public void Update(int stateHash)
        {
            if (stateHash == idleState)
            {
                state = UnityChanAnimatorState.Idle;
            }
            else if (stateHash == locoState)
            {
                state = UnityChanAnimatorState.Locomotion;
            }
            else if (stateHash == jumpState)
            {
                state = UnityChanAnimatorState.Jump;
            }
            else if (stateHash == restState)
            {
                state = UnityChanAnimatorState.Rest;
            }
        }
    }
}