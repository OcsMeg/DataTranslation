using UnityEngine;
using System.Collections;
using Photon.Pun;
using UnityEngine.XR;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    [SerializeField] private GameObject RHandTarget;

    // カードオブジェクト
    private GameObject card;

    // カード生成済みかどうか
    private bool isCreated = false;

    private PhotonView photonView;

    // XR右コントローラー
    private InputDevice rightController;

    // ボタンの前フレーム値
    private bool prevBPressed = false;

    private string userID;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        GetRightController();
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        if (!rightController.isValid)
            GetRightController();

        // Bボタン（右手の secondaryButton）
        bool bPressed = false;
        if (rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool pressed))
        {
            bPressed = pressed;
        }

        // GetDown と同じ動作（押した瞬間のみ）
        if (bPressed && !prevBPressed)
        {
            ToggleCard();
        }

        prevBPressed = bPressed;

        // PC 用テストキー（任意）
        if (Input.GetKeyDown(KeyCode.Y))
        {
            ToggleCard();
        }
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
    /// カードの生成、または表示/非表示切り替え
    /// </summary>
    private void ToggleCard()
    {
        if (!isCreated)
        {
            // 初回：カード生成
            userID = PlayerMode.GetPlayerName();

            card = PhotonNetwork.Instantiate(
                "ReceivingCard",
                RHandTarget.transform.position,
                Quaternion.identity,
                0,
                new object[] { userID }
            );

            // 追従スクリプトにターゲットを渡す
            TrackingRHand trackingRHand = card.GetComponent<TrackingRHand>();
            trackingRHand.TrackCard(RHandTarget);

            isCreated = true;
        }
        else
        {
            // 2回目以降：SetActive のトグル
            if (card != null)
            {
                card.SetActive(!card.activeSelf);
            }
        }
    }
}
