using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR;

public class CardManager : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private GameObject RHandTarget;
    [SerializeField] private HandGestureDetector rightHandDetector;

    // 追加：DataSharing参照（同一プレイヤー内の共有状態を見る）
    [SerializeField] private DataSharing dataSharing;

    private GameObject card;
    private bool isCreated = false;

    private PhotonView photonView;

    private InputDevice rightController;
    private bool prevBPressed = false;

    private string userID;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();

        // 同じオブジェクト or 親から探す（どっちでも拾えるように）
        // dataSharing = GetComponent<DataSharing>();
        // if (dataSharing == null) dataSharing = GetComponentInParent<DataSharing>();
    }

    void OnEnable()
    {
        if (rightHandDetector != null)
        {
            rightHandDetector.OnFist += HandleRightHandFist;
        }
    }

    void OnDisable()
    {
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
        if (!photonView.IsMine) return;

        if (ShareMode.GetShareMethod() != ShareMode.ShareMethod.Hover)
            return;

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

        if (Input.GetKeyDown(KeyCode.Y))
        {
            ToggleCardCreateOrDestroy();
        }
    }

    private void HandleRightHandFist()
    {
        if (!photonView.IsMine) return;
        if (ShareMode.GetShareMethod() != ShareMode.ShareMethod.Hover) return;

        ToggleCardCreateOrDestroy();
    }

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

    private void ToggleCardCreateOrDestroy()
    {
        // まだない → 生成
        if (!isCreated || card == null)
        {
            // 追加：共有中なら生成しない（破棄は許可）
            if (dataSharing != null && dataSharing.IsSharing)
            {
                Debug.Log("[CardManager] 共有中のためカード生成を抑制しました");
                return;
            }

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

            TrackingRHand trackingRHand = card.GetComponent<TrackingRHand>();
            if (trackingRHand != null && RHandTarget != null)
            {
                trackingRHand.TrackCard(RHandTarget);
            }

            isCreated = true;
        }
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
