using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// 入力（Xボタン / 左手グー）を監視して ImageLoader の表示を制御する
/// </summary>
public class DisplayToggleController : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private ImageLoader imageLoader;
    [SerializeField] private HandGestureDetector leftHandDetector;  // 左手用（Use Right Hand = false）

    // XR 左コントローラー（Xボタン用）
    private InputDevice leftController;
    private bool prevXPressed = false;

    private PhotonView photonView;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        GetLeftController();
    }

    void OnEnable()
    {
        if (leftHandDetector != null)
        {
            leftHandDetector.OnFist += OnLeftHandFist;
        }
    }

    void OnDisable()
    {
        if (leftHandDetector != null)
        {
            leftHandDetector.OnFist -= OnLeftHandFist;
        }
    }

    void Update()
    {
        // 自分の Avatar でなければ入力を無視
        if (photonView != null && !photonView.IsMine) return;

        if (!leftController.isValid)
            GetLeftController();

        bool xPressedNow = false;

        // XR コントローラーの Xボタン状態を取得
        if (leftController.TryGetFeatureValue(CommonUsages.primaryButton, out bool xPressed))
            xPressedNow = xPressed;

        bool xButtonDown = (xPressedNow && !prevXPressed);
        bool pKeyDown = Input.GetKeyDown(KeyCode.P); // デバッグ用

        if (xButtonDown || pKeyDown)
        {
            ToggleCanvas();
        }

        prevXPressed = xPressedNow;
    }

    /// <summary>
    /// 左手がグーになったときに呼ばれる
    /// </summary>
    private void OnLeftHandFist()
    {
        if (photonView != null && !photonView.IsMine) return;
        ToggleCanvas();
    }

    private void ToggleCanvas()
    {
        if (imageLoader == null) return;
        imageLoader.ToggleDisplay();
    }

    /// <summary>
    /// 左コントローラー取得（Xボタンを使うため）
    /// </summary>
    private void GetLeftController()
    {
        var devices = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller,
            devices);

        if (devices.Count > 0)
        {
            leftController = devices[0];
            Debug.Log("左コントローラー取得 (DisplayToggleController)");
        }
    }
}
