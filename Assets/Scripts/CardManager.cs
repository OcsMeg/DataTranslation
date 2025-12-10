using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR;

public class CardManager : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private GameObject RHandTarget;              // 右手の追従ターゲット
    [SerializeField] private HandGestureDetector rightHandDetector; // 右手用ジェスチャ検出（useRightHand = true にしておく）

    private GameObject card;     // 生成したカード
    private bool isCreated = false;

    private PhotonView photonView;

    // XR右コントローラー
    private InputDevice rightController;
    private bool prevBPressed = false;

    private string userID;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    void OnEnable()
    {
        // 右手のグーイベントに登録
        if (rightHandDetector != null)
        {
            rightHandDetector.OnFist += HandleRightHandFist;
        }
    }

    void OnDisable()
    {
        // イベント解除（シーン遷移などで重複登録しないように）
        if (rightHandDetector != null)
        {
            rightHandDetector.OnFist -= HandleRightHandFist;
        }
    }

    void Start()
    {
        GetRightController();
    }

    void Update()
    {
        // 自分のプレイヤー以外は無視
        if (!photonView.IsMine) return;

        // Hover 以外のモードでは何もしない
        if (ShareMode.GetShareMethod() != ShareMode.ShareMethod.Hover)
            return;

        // --- コントローラ B ボタンでのトグル ---
        if (!rightController.isValid)
            GetRightController();

        bool bPressedNow = false;
        if (rightController.isValid &&
            rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool pressed))
        {
            bPressedNow = pressed;
        }

        if (bPressedNow && !prevBPressed)
        {
            ToggleCardCreateOrDestroy();
        }

        prevBPressed = bPressedNow;

        // PC テスト用キー
        if (Input.GetKeyDown(KeyCode.Y))
        {
            ToggleCardCreateOrDestroy();
        }
    }

    /// <summary>
    /// 右手がグーになった瞬間に呼ばれる（HandGestureDetector.OnFist）
    /// </summary>
    private void HandleRightHandFist()
    {
        // 念のためオーナーチェック & ShareMode チェック
        if (!photonView.IsMine) return;
        if (ShareMode.GetShareMethod() != ShareMode.ShareMethod.Hover) return;

        ToggleCardCreateOrDestroy();
    }

    /// <summary>
    /// XR右コントローラー取得
    /// </summary>
    private void GetRightController()
    {
        var devices = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller,
            devices
        );

        if (devices.Count > 0)
        {
            rightController = devices[0];
            Debug.Log("[CardManager] 右コントローラー取得完了");
        }
    }

    /// <summary>
    /// カードがなければ生成して右手に追従、あれば破棄
    /// </summary>
    private void ToggleCardCreateOrDestroy()
    {
        // まだない → 生成
        if (!isCreated || card == null)
        {
            userID = PlayerMode.GetPlayerName();

            Vector3 spawnPos = RHandTarget != null
                ? RHandTarget.transform.position
                : transform.position;

            Quaternion spawnRot = RHandTarget != null
                ? RHandTarget.transform.rotation
                : Quaternion.identity;

            card = PhotonNetwork.Instantiate(
                "ReceivingCard",
                spawnPos,
                spawnRot,
                0,
                new object[] { userID }
            );

            // 追従スクリプトにターゲットを渡す
            TrackingRHand trackingRHand = card.GetComponent<TrackingRHand>();
            if (trackingRHand != null && RHandTarget != null)
            {
                trackingRHand.TrackCard(RHandTarget);
            }

            isCreated = true;
        }
        // 既にある → 破棄
        else
        {
            if (card != null)
            {
                PhotonNetwork.Destroy(card);
                card = null;
            }
            isCreated = false;
        }
    }
}
