using UnityEngine;

namespace UnityChan.Rx
{
    public class DeltaTime : IDeltaTime
    {
        public float delta { get { return Time.deltaTime; } }
    }
}