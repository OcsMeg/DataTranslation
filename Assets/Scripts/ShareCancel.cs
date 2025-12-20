using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using TMPro;

public class ShareCancel : MonoBehaviour
{
    [SerializeField] private DataSharing dataSharing;
    [SerializeField] private HoverMethod hoverMethod;
    [SerializeField] private GUIMethod guiMethod;
    [SerializeField] private ControllerRaySelect rayMethod;

    private InputDevice rightController;
    private bool prevGripPressed = false; // 中指グリップ（共有終了ボタン）

    [SerializeField] private HandGestureDetector handGesture;
    
    [SerializeField] private TextMeshProUGUI debugText;

    void Awake()
    {
        GetRightController();
    }

    void OnEnable()
    {
        if (handGesture != null)
        {
            handGesture.OnScissors += HandleScissors;
        }
    }

    void OnDisable()
    {
        if (handGesture != null)
        {
            handGesture.OnScissors -= HandleScissors;
        }
    }

    void Update()
    {
        //共有中じゃないと処理しない
        if (!dataSharing.IsSharing) return;
        
        // VRモード以外は処理しない
        if (PlayerMode.PlayMode.VR != PlayerMode.GetSelectedPlayMode()) return;
        
        // コントローラーが無効になったら再取得
        if (!rightController.isValid)
        {
            GetRightController();
        }

        // 中指グリップ
        rightController.TryGetFeatureValue(CommonUsages.gripButton, out bool gripPressed);

        if (gripPressed && !prevGripPressed)
        {
            CancelIsSharing();
        }

        prevGripPressed = gripPressed;
    }

    public void HandleScissors()
    {
        // 念のため共有中のときだけキャンセル
        if (dataSharing != null && dataSharing.IsSharing)
        {
            CancelIsSharing();
        }
        debugText.text = "Scissors";
    }

    private void CancelIsSharing()
    {
        if (ShareMode.ShareMethod.Hover == ShareMode.GetShareMethod())
        {
            hoverMethod.DestroySharingCard();
        }
        else if (ShareMode.ShareMethod.Ray == ShareMode.GetShareMethod())
        {
            rayMethod.DeactivateRay();
        }
        else if (ShareMode.ShareMethod.GUI == ShareMode.GetShareMethod())
        {
            guiMethod.CancelShare();
        }

        //dataSharingのisSharingをfalseにする
        dataSharing.StopSharing();
    }

    private void GetRightController()
    {
        var devices = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller,
            devices);

        if (devices.Count > 0)
        {
            rightController = devices[0];
            Debug.Log($"[ControllerRaySelect] 右コントローラー取得: {rightController.name}");
        }
        else
        {
            Debug.LogWarning("[ControllerRaySelect] 右コントローラーが見つかりませんでした");
        }
    }
}
