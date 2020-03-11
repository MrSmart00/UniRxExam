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
		// キャラクターコントローラ（カプセルコライダ）の参照
		private CapsuleCollider col;
		private Rigidbody rb;
		// キャラクターコントローラ（カプセルコライダ）の移動量
		private Vector3 velocity;
		// CapsuleColliderで設定されているコライダのHeiht、Centerの初期値を収める変数
		private float orgColHight;
		private Vector3 orgVectColCenter;
		private Animator anim;                          // キャラにアタッチされるアニメーターへの参照
		private AnimatorStateInfo currentBaseState;         // base layerで使われる、アニメーターの現在の状態の参照

		private GameObject cameraObject;    // メインカメラへの参照

        [Inject]
		public ICharacterMover mover;

		[Inject]
		public IAnimatorStatus<UnityChanAnimatorState> status;

        [Inject]
		private IUnityChanViewModel<UnityChanAnimatorState, UnityChanRxViewModelContext> viewModel;

		// 初期化
		void Start()
		{
			viewModel.inject(
                new UnityChanRxViewModelContext(
                    forwardSpeed: forwardSpeed,
                    backwardSpeed: backwardSpeed,
                    rotateSpeed: rotateSpeed,
                    jumpPower: jumpPower,
                    transform: transform
                    )
                );

			// Animatorコンポーネントを取得する
			anim = GetComponent<Animator>();
			anim.speed = animSpeed;                             // Animatorのモーション再生速度に animSpeedを設定する

			// CapsuleColliderコンポーネントを取得する（カプセル型コリジョン）
			col = GetComponent<CapsuleCollider>();
			rb = GetComponent<Rigidbody>();
			//メインカメラを取得する
			cameraObject = GameObject.FindWithTag("MainCamera");
			// CapsuleColliderコンポーネントのHeight、Centerの初期値を保存する
			orgColHight = col.height;
			orgVectColCenter = col.center;

			var input = new UnityChanViewModelInput(
                update: this.FixedUpdateAsObservable(),
                stateInfo: anim.GetBehaviour<ObservableStateMachineTrigger>().OnStateUpdateAsObservable());
			var output = viewModel.transform(input: input);

			output
				.state
				.Subscribe(state =>
				{
                    
				});
			output
				.userInput
				.Subscribe(_ =>
				{
					anim.SetFloat("Speed", _.speed);
					anim.SetFloat("Direction", _.direction);
                    if(_.isJump)
                    {
						anim.SetBool("Jump", true);
                    }
				});
            output
                .move
                .Subscribe(_ =>
                {
					transform.localPosition = _.position;
					transform.Rotate(_.rotate);
                    if(_.jump != Vector3.zero)
                    {
						rb.AddForce(_.jump, ForceMode.VelocityChange);
                    }
                });
        }


		// 以下、メイン処理.リジッドボディと絡めるので、FixedUpdate内で処理を行う.
		void FixedUpdate()
		{

			currentBaseState = anim.GetCurrentAnimatorStateInfo(0); // 参照用のステート変数にBase Layer (0)の現在のステートを設定する
			rb.useGravity = true;//ジャンプ中に重力を切るので、それ以外は重力の影響を受けるようにする
			status.update(currentBaseState.fullPathHash);
 
			// 以下、Animatorの各ステート中での処理
			// Locomotion中
			// 現在のベースレイヤーがlocoStateの時
			if (status.currentState == UnityChanAnimatorState.Locomotion)
			{
				//カーブでコライダ調整をしている時は、念のためにリセットする
				if (useCurves)
				{
					resetCollider();
				}
			}
			// JUMP中の処理
			// 現在のベースレイヤーがjumpStateの時
			else if (status.currentState == UnityChanAnimatorState.Jump)
			{
				//cameraObject.SendMessage("setCameraPositionJumpView");  // ジャンプ中のカメラに変更
				//														// ステートがトランジション中でない場合
				if (!anim.IsInTransition(0))
				{

					// 以下、カーブ調整をする場合の処理
					if (useCurves)
					{
						// 以下JUMP00アニメーションについているカーブJumpHeightとGravityControl
						// JumpHeight:JUMP00でのジャンプの高さ（0〜1）
						// GravityControl:1⇒ジャンプ中（重力無効）、0⇒重力有効
						float jumpHeight = anim.GetFloat("JumpHeight");
						float gravityControl = anim.GetFloat("GravityControl");
						if (gravityControl > 0)
							rb.useGravity = false;  //ジャンプ中の重力の影響を切る

						// レイキャストをキャラクターのセンターから落とす
						Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
						RaycastHit hitInfo = new RaycastHit();
						// 高さが useCurvesHeight 以上ある時のみ、コライダーの高さと中心をJUMP00アニメーションについているカーブで調整する
						if (Physics.Raycast(ray, out hitInfo))
						{
							if (hitInfo.distance > useCurvesHeight)
							{
								col.height = orgColHight - jumpHeight;          // 調整されたコライダーの高さ
								float adjCenterY = orgVectColCenter.y + jumpHeight;
								col.center = new Vector3(0, adjCenterY, 0); // 調整されたコライダーのセンター
							}
							else
							{
								// 閾値よりも低い時には初期値に戻す（念のため）					
								resetCollider();
							}
						}
					}
					// Jump bool値をリセットする（ループしないようにする）				
					anim.SetBool("Jump", false);
				}
			}
			// IDLE中の処理
			// 現在のベースレイヤーがidleStateの時
			else if (status.currentState == UnityChanAnimatorState.Idle)
			{
				//カーブでコライダ調整をしている時は、念のためにリセットする
				if (useCurves)
				{
					resetCollider();
				}
				// スペースキーを入力したらRest状態になる
				if (mover.GetJump())
				{
					anim.SetBool("Rest", true);
				}
			}
			// REST中の処理
			// 現在のベースレイヤーがrestStateの時
			else if (status.currentState == UnityChanAnimatorState.Rest)
			{
				//cameraObject.SendMessage("setCameraPositionFrontView");		// カメラを正面に切り替える
				// ステートが遷移中でない場合、Rest bool値をリセットする（ループしないようにする）
				if (!anim.IsInTransition(0))
				{
					anim.SetBool("Rest", false);
				}
			}
		}

		// キャラクターのコライダーサイズのリセット関数
		void resetCollider()
		{
			// コンポーネントのHeight、Centerの初期値を戻す
			col.height = orgColHight;
			col.center = orgVectColCenter;
		}
	}
}