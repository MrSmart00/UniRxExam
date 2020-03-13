using UnityEngine;

namespace UnityChan.Rx
{
    public struct UnityChanLocomotionModel
    {
        float forwardSpeed;
        float backwardSpeed;
        float rotateSpeed;
        float jumpPower;
        float jumpThreshold;
        float colliderOffsetY;
        float colliderDefaultHeight;
        Transform transform;
        float deltaTime;

        public UnityChanLocomotionModel(float forwardSpeed, float backwardSpeed, float rotateSpeed, float jumpPower, float jumpThreshold, float colliderOffsetY, float colliderDefaultHeight, Transform transform, float deltaTime)
        {
            this.forwardSpeed = forwardSpeed;
            this.backwardSpeed = backwardSpeed;
            this.rotateSpeed = rotateSpeed;
            this.jumpPower = jumpPower;
            this.jumpThreshold = jumpThreshold;
            this.colliderOffsetY = colliderOffsetY;
            this.colliderDefaultHeight = colliderDefaultHeight;
            this.transform = transform;
            this.deltaTime = deltaTime;
        }

        public (Vector3 position, Vector3 rotate, Vector3 jump) convertVelocity(float horizontal, float vertical, bool isJump)
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
                rotate: new Vector3(x: 0, y: horizontal * rotateSpeed, z: 0),
                jump: isJump ? Vector3.up * jumpPower : Vector3.zero
                );
        }

        public bool needsGravity(float control)
        {
            if (control > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public (float centerY, float height) calculateCurveDelta(float reduce)
        {
            // レイキャストをキャラクターのセンターから落とす
            Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
            RaycastHit hitInfo = new RaycastHit();
            // 高さが useCurvesHeight 以上ある時のみ、コライダーの高さと中心をJUMP00アニメーションについているカーブで調整する
            if (Physics.Raycast(ray, out hitInfo) && hitInfo.distance > jumpThreshold)
            {
                return (centerY: colliderOffsetY + reduce, height: colliderDefaultHeight - reduce);
            }
            else
            {
                return (centerY: colliderOffsetY, height: colliderDefaultHeight);
            }
        }
    }
}