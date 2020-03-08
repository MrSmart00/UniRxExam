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
            Assert.AreEqual(status.CurrentState(), UnityChanAnimatorState.Idle);
        }

        [Test]
        public void ステータス変更確認()
        {
            status.Update(Animator.StringToHash("Base Layer.Locomotion"));
            Assert.AreEqual(status.CurrentState(), UnityChanAnimatorState.Locomotion);
            status.Update(Animator.StringToHash("Base Layer.Jump"));
            Assert.AreEqual(status.CurrentState(), UnityChanAnimatorState.Jump);
            status.Update(Animator.StringToHash("Base Layer.Rest"));
            Assert.AreEqual(status.CurrentState(), UnityChanAnimatorState.Rest);
            status.Update(Animator.StringToHash("Base Layer.Idle"));
            Assert.AreEqual(status.CurrentState(), UnityChanAnimatorState.Idle);
        }

        [Test]
        public void 異常値入力確認()
        {
            status.Update(-1);
            Assert.AreEqual(status.CurrentState(), UnityChanAnimatorState.Idle);
        }
    }
}
