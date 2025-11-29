using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using System.Collections.Generic;

public class ModeSelectManager : MonoBehaviour
{
    private bool isModeSelected = false;
    [SerializeField] private SelectModeUI selectedModeUI;

    private int selectIndex = 0;

    private LoginManager loginManager;
    [SerializeField] private Canvas loginCanvas;
    [SerializeField] private Canvas modeSelectCanvas;

    private PlayerMode.PlayMode[] modelists = new PlayerMode.PlayMode[]
    {
        PlayerMode.PlayMode.VR,
        PlayerMode.PlayMode.MR
    };

    // XRコントローラー
    private InputDevice leftController;
    private InputDevice rightController;

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

        // 左コントローラー Grip Button で左移動
        if (leftController.TryGetFeatureValue(CommonUsages.gripButton, out bool leftGrip) && leftGrip)
        {
            MoveSelectionLeft();
        }

        // 右コントローラー Grip Button で右移動
        if (rightController.TryGetFeatureValue(CommonUsages.gripButton, out bool rightGrip) && rightGrip)
        {
            MoveSelectionRight();
        }

        // 右コントローラー Aボタン（primaryButton）で決定
        if (rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool aPressed) && aPressed)
        {
            SelectMode(modelists[selectIndex]);
        }

        // PCデバッグ用 GODモード
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SelectMode(PlayerMode.PlayMode.GOD);
            
        }

        // PCデバッグ用 左右
        if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveSelectionLeft();
        if (Input.GetKeyDown(KeyCode.RightArrow)) MoveSelectionRight();
        if (Input.GetKeyDown(KeyCode.Return)) SelectMode(modelists[selectIndex]);
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
            selectIndex = modelists.Length - 1;

        UpdateSelectionUI();
        Debug.Log($"Current Selection: {modelists[selectIndex]}");
    }

    private void MoveSelectionRight()
    {
        selectIndex++;
        if (selectIndex >= modelists.Length)
            selectIndex = 0;

        UpdateSelectionUI();
        Debug.Log($"Current Selection: {modelists[selectIndex]}");
    }

    private void UpdateSelectionUI()
    {
        switch (modelists[selectIndex])
        {
            case PlayerMode.PlayMode.VR:
                selectedModeUI.VRSelected();
                break;
            case PlayerMode.PlayMode.MR:
                selectedModeUI.MRSelected();
                break;
        }
    }

    private void SelectMode(PlayerMode.PlayMode mode)
    {
        if (isModeSelected) return; // 二重実行防止
        isModeSelected = true;

        PlayerMode.SetSelectedPlayMode(mode);

        Debug.Log($"Selected Mode: {mode}");

        modeSelectCanvas.enabled = false;
        loginCanvas.enabled = true;

        if (PlayerMode.GetSelectedPlayMode() == PlayerMode.PlayMode.GOD)
        {
            SceneManager.LoadScene("PlayScene");
        }

        loginManager.LoginStart();
    }
}
