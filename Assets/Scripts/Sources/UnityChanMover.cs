﻿using UnityEngine;

namespace UnityChan.Rx
{
    public class UnityChanMover : ICharacterMover
    {
        public float HorizontalAxis()
        {
            return Input.GetAxis("Horizontal");
        }

        public bool IsJumping()
        {
            return Input.GetButtonDown("Jump");
        }

        public float VerticalAxis()
        {
            return Input.GetAxis("Vertical");
        }
    }
}