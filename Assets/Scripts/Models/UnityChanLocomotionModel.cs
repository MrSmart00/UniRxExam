using UnityEngine;

namespace UnityChan.Rx
{
    public struct UnityChanLocomotionModel
    {
        float forwardSpeed;
        float backwardSpeed;
        float rotateSpeed;
        Transform transform;
        float deltaTime;

        public UnityChanLocomotionModel(float forwardSpeed, float backwardSpeed, float rotateSpeed, Transform transform, float deltaTime)
        {
            this.forwardSpeed = forwardSpeed;
            this.backwardSpeed = backwardSpeed;
            this.rotateSpeed = rotateSpeed;
            this.transform = transform;
            this.deltaTime = deltaTime;
        }

        public (Vector3 position, Vector3 rotate) convertVelocity(float horizontal, float vertical)
        {
            // 上下のキー入力からZ軸方向の移動量を取得
            // キャラクターのローカル空間での方向に変換
            var velocity = transform.TransformDirection(new Vector3(0, 0, vertical));
            //以下のvの閾値は、Mecanim側のトランジションと一緒に調整する
            if (vertical > 0.1)
            {
                velocity *= forwardSpeed;       // 移動速度を掛ける
            }
            else if (vertical < -0.1)
            {
                velocity *= backwardSpeed;  // 移動速度を掛ける
            }

            return (
                position: transform.localPosition + (velocity * deltaTime),
                rotate: new Vector3(x: 0, y: horizontal * rotateSpeed, z: 0)
                );
        }
    }
}