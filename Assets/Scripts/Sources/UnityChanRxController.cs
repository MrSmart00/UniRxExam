using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Triggers;

namespace UnityChan.Rx
{
	// 必要なコンポーネントの列記
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Rigidbody))]

	public class UnityChanRxController : MonoBehaviour
	{

		public float animSpeed = 1.5f;              // アニメーション再生速度設定
		public float lookSmoother = 3.0f;           // a smoothing setting for camera motion
		public bool useCurves = true;               // Mecanimでカーブ調整を使うか設定する
													// このスイッチが入っていないとカーブは使われない
		public float useCurvesHeight = 0.5f;        // カーブ補正の有効高さ（地面をすり抜けやすい時には大きくする）

		// 以下キャラクターコントローラ用パラメタ
		// 前進速度
		public float forwardSpeed = 7.0f;
		// 後退速度
		public float backwardSpeed = 2.0f;
		// 旋回速度
		public float rotateSpeed = 2.0f;
		// ジャンプ威力
		public float jumpPower = 3.0f;

		ReactiveProperty<float> jumpHeight;
		ReactiveProperty<float> gravityControl;

        [Inject]
		private IViewModel<ViewModelContext, ViewModelInput, ViweModelOutput<UnityChanAnimatorState>> viewModel;

		void Start()
		{
			// CapsuleColliderコンポーネントを取得する（カプセル型コリジョン）
			var col = GetComponent<CapsuleCollider>();
			// Animatorコンポーネントを取得する
			var anim = GetComponent<Animator>();
			anim.speed = animSpeed;                             // Animatorのモーション再生速度に animSpeedを設定する
			var rb = GetComponent<Rigidbody>();

			viewModel.inject(
                new ViewModelContext(
                    forwardSpeed: forwardSpeed,
                    backwardSpeed: backwardSpeed,
                    rotateSpeed: rotateSpeed,
                    jumpPower: jumpPower,
                    transform: transform,
                    useCurves: useCurves,
                    useCurvesHeight: useCurvesHeight,
                    colliderCenterY: col.center.y,
                    colliderHight: col.height
                    )
                );

			jumpHeight = new ReactiveProperty<float>(anim.GetFloat("JumpHeight"));
			gravityControl = new ReactiveProperty<float>(anim.GetFloat("GravityControl"));

			var input = new ViewModelInput(
                update: this.FixedUpdateAsObservable(),
                stateInfo: anim.GetBehaviour<ObservableStateMachineTrigger>().OnStateUpdateAsObservable(),
                jumpHeight: jumpHeight,
                gravityControl: gravityControl
                );

			var output = viewModel.transform(input: input);

			output
				.state
				.Subscribe(_ =>
				{
                    if (_ == UnityChanAnimatorState.Jump && useCurves)
                    {
						jumpHeight.Value = anim.GetFloat("JumpHeight");
						gravityControl.Value = anim.GetFloat("GravityControl");
					}
				});

			output
				.needsGravity
				.Subscribe(_ => rb.useGravity = _);

			output
				.colliderInfo
				.Subscribe(_ =>
				{
					col.height = _.height;
					col.center = new Vector3(0, _.centerY, 0);
				});

			output
				.userInput
				.Subscribe(_ =>
				{
					anim.SetFloat("Speed", _.speed);    // Animator側で設定している"Speed"パラメタにvを渡す
					anim.SetFloat("Direction", _.direction);    // Animator側で設定している"Direction"パラメタにhを渡す
					if (_.isJump)
                    {
						anim.SetTrigger("Jump");        // Animatorにジャンプに切り替えるフラグを送る
					}
                    else if (_.isRest)
                    {
						anim.SetTrigger("Rest");
					}
				});
            output
                .move
                .Subscribe(_ =>
                {
					// 上下のキー入力でキャラクターを移動させる
					transform.localPosition = _.position;
					// 左右のキー入力でキャラクタをY軸で旋回させる
					transform.Rotate(_.rotate);
                    if(_.jump != Vector3.zero)
                    {
						rb.AddForce(_.jump, ForceMode.VelocityChange);
                    }
				});
        }
	}
}