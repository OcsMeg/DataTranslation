using Photon.Pun;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("各種設定")]
    [Tooltip("移動速度")]
    public float moveSpeed = 2.0f;

    [Tooltip("アニメーションを開始する速度のしきい値")]
    public float animationThreshold = 0.1f;

    [Header("トラッキング対象オブジェクト")]
    [Tooltip("HMD（OVRCameraRig内のCenterEyeAnchorなど）")]

    // --- 内部変数 ---
    private CharacterController characterController;

    void Start()
    {
        
    }

    void Update()
    {
        // スティックでの移動処理
        HandleStickMovement();
    }
    /// <summary>
    /// コントローラのスティック入力による移動処理
    /// </summary>
    private void HandleStickMovement()
    {
        
        // 左スティックの入力を取得
        Vector2 stickInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);

        if (stickInput.magnitude > 0.1f) // 少し倒しただけで動かないようにデッドゾーンを設ける
        {
            // HMDが向いている方向を基準に移動方向を決定
            Vector3 moveDirection = new Vector3(stickInput.x, 0, stickInput.y);
            Vector3 worldMoveDirection = transform.TransformDirection(moveDirection); // キャラクターの向きを基準に変換

            // CharacterControllerで移動させる
            transform.Translate(worldMoveDirection * moveSpeed * Time.deltaTime);

        }
        
        // 右スティックの入力を取得（視点回転用）
        // Vector2 rightStickInput = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick, OVRInput.Controller.RTouch);
        //
        // if (rightStickInput.magnitude > 0.1f)
        // {
        //     // 水平方向の回転（Y軸周り）
        //     float rotationSpeed = 60.0f; // 1秒あたりの回転角度
        //     float horizontalRotation = rightStickInput.x * rotationSpeed * Time.deltaTime;
        //
        //     // 視点の回転
        //     transform.Rotate(0, horizontalRotation, 0, Space.World);
        //
        //     // オプション: 垂直方向の回転を追加したい場合（上下の視点移動）
        //     // 注意: VR酔いを防ぐため、垂直方向の回転は制限するか、無効にすることが一般的です
        //     float maxVerticalAngle = 30.0f; // 最大角度制限
        //     float verticalRotation = -rightStickInput.y * rotationSpeed * 0.5f * Time.deltaTime; // 垂直回転は少し遅くする
        //
        //     // 現在の角度を確認して制限を適用
        //     float currentXRotation = transform.eulerAngles.x;
        //     // 角度を-180〜180の範囲に変換
        //     if (currentXRotation > 180) currentXRotation -= 360;
        //
        //     // 新しい角度が制限内に収まるか確認
        //     float newXRotation = currentXRotation + verticalRotation;
        //     if (newXRotation > -maxVerticalAngle && newXRotation < maxVerticalAngle)
        //     {
        //         // 垂直方向（X軸周り）の回転を適用
        //         transform.Rotate(verticalRotation, 0, 0);
        //     }
        //     
        // }
    }
}
