//
// Mecanimのアニメーションデータが、原点で移動しない場合の Rigidbody付きコントローラ
// VR対応改造版
//
using UnityEngine;
using System.Collections;

namespace UnityChan
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerControler : MonoBehaviour
    {
        public float animSpeed = 1.5f; // アニメーション再生速度設定

        // --- VR用に変更・追加したパラメータ ---
        [Header("VR 設定")]
        [Tooltip("スティックによる移動を許可するかどうか")]
        public bool canMoveByStick = true;
        [Tooltip("HMD（OVRCameraRig内のCenterEyeAnchorなど）")]
        public Transform headTarget;
        [Tooltip("左コントローラ（OVRCameraRig内のLeftHandAnchorなど）")]
        public Transform leftHandTarget;
        [Tooltip("右コントローラ（OVRCameraRig内のRightHandAnchorなど）")]
        public Transform rightHandTarget;
        
        // --- 元のスクリプトのパラメータ（一部流用） ---
        public float forwardSpeed = 7.0f; // 前進速度
        public float rotateSpeed = 2.0f; // 旋回速度（VRでは自動追従）

        // --- 内部変数 ---
        private Rigidbody rb;
        private Animator anim;
        private Vector3 previousHeadPosition; // HMDの物理移動速度を計算するための変数
        
        // 手の回転をモデルに合わせるための補正値
        private readonly Quaternion rightHandRotationCorrection = Quaternion.Euler(0f, 0f, -90f);
        private readonly Quaternion leftHandRotationCorrection = Quaternion.Euler(0f, 0f, 90f);

        // 初期化
        void Start()
        {
            anim = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
            
            // Rigidbodyの制約を設定（物理演算で勝手に回転しないように）
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            // VR用の初期化
            if (headTarget != null)
            {
                previousHeadPosition = headTarget.position;
            }
            else
            {
                Debug.LogError("Head Targetが設定されていません。");
            }
        }

        // 物理演算系の処理はFixedUpdateで行う
        void FixedUpdate()
        {
            if (headTarget == null) return;

            // --- 1. キャラクターの回転をHMDに追従させる ---
            rb.MoveRotation(Quaternion.Euler(0, headTarget.eulerAngles.y, 0));

            // --- 2. キャラクターの位置を更新する ---
            // HMDの水平位置を基準とする
            Vector3 targetPosition = new Vector3(headTarget.position.x, rb.position.y, headTarget.position.z);

            // スティック入力による移動量を計算
            Vector2 stickInput = canMoveByStick ? OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch) : Vector2.zero;
            Vector3 stickMovement = (transform.forward * stickInput.y + transform.right * stickInput.x) * forwardSpeed * Time.fixedDeltaTime;

            // HMDの位置とスティックの移動量を合成して移動
            rb.MovePosition(targetPosition + stickMovement);
            
            // --- 3. アニメーションを更新する ---
            // HMDの物理的な移動速度（水平方向）を計算
            Vector3 hmdHorizontalVelocity = (new Vector3(headTarget.position.x, 0, headTarget.position.z) - new Vector3(previousHeadPosition.x, 0, previousHeadPosition.z)) / Time.fixedDeltaTime;
            previousHeadPosition = headTarget.position;
            
            // HMDの物理移動とスティック入力の大きい方を速度として採用
            float moveMagnitude = Mathf.Max(hmdHorizontalVelocity.magnitude, stickInput.magnitude);

            anim.SetFloat("Speed", moveMagnitude);
            anim.SetFloat("Direction", 0); // VRでは左右移動は使わないため0に固定
            anim.speed = animSpeed;
        }

        // --- IK (インバースキネマティクス) 処理をここに追加 ---
        void OnAnimatorIK(int layerIndex)
        {
            if (anim == null || headTarget == null || leftHandTarget == null || rightHandTarget == null) return;

            // 頭のIK
            anim.SetLookAtWeight(1.0f);
            anim.SetLookAtPosition(headTarget.position);

            // 右手のIK
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
            anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
            anim.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation * rightHandRotationCorrection);

            // 左手のIK
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation * leftHandRotationCorrection);
        }

        // 元のOnGUI()はVRでは表示されないため削除
        // 元のresetCollider()はジャンプ処理用だったため削除
    }
}