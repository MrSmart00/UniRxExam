using UnityEngine;

namespace UnityChan.Rx
{
    public class UnityChanMover : ICharacterMover
    {
        public float GetHorizontalAxis()
        {
            return Input.GetAxis("Horizontal");
        }

        public bool GetJump()
        {
            return Input.GetButtonDown("Jump");
        }

        public float GetVerticalAxis()
        {
            return Input.GetAxis("Vertical");
        }
    }
}
