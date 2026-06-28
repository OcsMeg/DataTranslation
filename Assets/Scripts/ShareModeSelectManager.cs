using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using System.Collections.Generic;

public class ShareModeSelectManager : MonoBehaviour
{
    private bool isModeSelected = false;
    [SerializeField] private SelectShareModeUI selectedShareModeUI;

    private int selectIndex = 0;

    private LoginManager loginManager;
    [SerializeField] private Canvas loginCanvas;
    [SerializeField] private Canvas shareModeSelectCanvas;

    private ShareMode.ShareMethod[] sharemodelists = new ShareMode.ShareMethod[]
    {
        ShareMode.ShareMethod.Hover,
        ShareMode.ShareMethod.Ray,
        ShareMode.ShareMethod.GUI
    };

    // XRコントローラー
    private InputDevice leftController;
    private InputDevice rightController;
    
    // 前フレームのボタン状態を保存するフラグ
    private bool prevLeftGrip = false;
    private bool prevRightGrip = false;
    private bool prevAPressed = false;
    private bool prevBPressed = false;

    void Start()
    {
        UpdateSelectionUI();

        loginCanvas.enabled = false;
        loginManager = GetComponent<LoginManager>();

        GetControllers();
    }

    void Update()
    {
        // XRデバイスが無効になったとき再取得
        if (!leftController.isValid || !rightController.isValid)
        {
            GetControllers();
        }

        // 現在のボタン状態を取得
        bool leftGrip = false;
        bool rightGrip = false;
        bool aPressed = false;
        bool bPressed = false;

        leftController.TryGetFeatureValue(CommonUsages.gripButton, out leftGrip);
        rightController.TryGetFeatureValue(CommonUsages.gripButton, out rightGrip);
        rightController.TryGetFeatureValue(CommonUsages.primaryButton, out aPressed);
        rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bPressed);

        // 左グリップ：false → true になった瞬間だけ左に移動
        if (leftGrip && !prevLeftGrip)
        {
            MoveSelectionLeft();
        }

        // 右グリップ：false → true になった瞬間だけ右に移動
        if (rightGrip && !prevRightGrip)
        {
            MoveSelectionRight();
        }

        // Bボタン：false → true になった瞬間だけ決定
        if (bPressed && !prevBPressed)
        {
            SelectMode(sharemodelists[selectIndex]);
        }

        // PCデバッグ用 GODモード
        // if (Input.GetKeyDown(KeyCode.Z))
        // {
        //     SelectMode(PlayerMode.PlayMode.GOD);
        // }

        // PCデバッグ用 左右
        if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveSelectionLeft();
        if (Input.GetKeyDown(KeyCode.RightArrow)) MoveSelectionRight();
        if (Input.GetKeyDown(KeyCode.Return)) SelectMode(sharemodelists[selectIndex]);

        // 最後に「今回の状態」を「前回の状態」として保存
        prevLeftGrip = leftGrip;
        prevRightGrip = rightGrip;
        prevAPressed = aPressed;
        prevBPressed = bPressed;
    }
    
    // XRコントローラー取得
    private void GetControllers()
    {
        var devices = new List<InputDevice>();

        // 左手
        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller,
            devices);

        if (devices.Count > 0)
            leftController = devices[0];

        // 右手
        devices.Clear();
        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller,
            devices);

        if (devices.Count > 0)
            rightController = devices[0];
    }

    private void MoveSelectionLeft()
    {
        selectIndex--;
        if (selectIndex < 0)
            selectIndex = sharemodelists.Length - 1;

        UpdateSelectionUI();
        Debug.Log($"Current Selection: {sharemodelists[selectIndex]}");
    }

    private void MoveSelectionRight()
    {
        selectIndex++;
        if (selectIndex >= sharemodelists.Length)
            selectIndex = 0;

        UpdateSelectionUI();
        Debug.Log($"Current Selection: {sharemodelists[selectIndex]}");
    }

    private void UpdateSelectionUI()
    {
        switch (sharemodelists[selectIndex])
        {
            case ShareMode.ShareMethod.Hover:
                selectedShareModeUI.HoverSelected();
                break;
            case ShareMode.ShareMethod.Ray:
                selectedShareModeUI.RaySelected();
                break;
            case ShareMode.ShareMethod.GUI:
                selectedShareModeUI.GUISelected();
                break;
        }
    }

    private void SelectMode(ShareMode.ShareMethod mode)
    {
        if (isModeSelected) return; // 二重実行防止
        isModeSelected = true;

        ShareMode.SetShareMethod(mode);

        Debug.Log($"Selected Mode: {mode}");

        shareModeSelectCanvas.enabled = false;
        loginCanvas.enabled = true;

        loginManager.LoginStart();
    }
}
