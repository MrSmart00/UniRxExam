using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityChan.Rx;

namespace Tests
{
    public class UnityChanAnimatorStatusTest
    {
        UnityChanAnimatorStatus status;

        [SetUp]
        public void Setup()
        {
            status = new UnityChanAnimatorStatus();
        }

        [Test]
        public void ステータス初期値確認()
        {
            Assert.AreEqual(status.currentState, UnityChanAnimatorState.Idle);
        }

        [Test]
        public void ステータス変更確認()
        {
            status.update(Animator.StringToHash("Base Layer.Locomotion"));
            Assert.AreEqual(status.currentState, UnityChanAnimatorState.Locomotion);
            status.update(Animator.StringToHash("Base Layer.Jump"));
            Assert.AreEqual(status.currentState, UnityChanAnimatorState.Jump);
            status.update(Animator.StringToHash("Base Layer.Rest"));
            Assert.AreEqual(status.currentState, UnityChanAnimatorState.Rest);
            status.update(Animator.StringToHash("Base Layer.Idle"));
            Assert.AreEqual(status.currentState, UnityChanAnimatorState.Idle);
        }

        [Test]
        public void 異常値入力確認()
        {
            status.update(-1);
            Assert.AreEqual(status.currentState, UnityChanAnimatorState.Idle);
        }
    }
}
