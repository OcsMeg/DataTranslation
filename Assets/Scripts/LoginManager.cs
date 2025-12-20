using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using UnityEngine.XR;
using System.Collections.Generic;

public class LoginManager : MonoBehaviour
{
    private bool isLogin = false;
    private bool isRequesting = false;  // 二重送信防止

    [SerializeField] private Canvas loginCanvas;
    [SerializeField] private TMP_InputField userIDField;
    [SerializeField] private TMP_InputField passwordField;

    private ApiClient api;

    // XRコントローラー
    private InputDevice rightController;

    void Start()
    {
        api = GetComponent<ApiClient>();
        GetRightController();
    }

    void Update()
    {
        if (!rightController.isValid)
            GetRightController();

        if (!isLogin) return;

        // Aボタン検知（右コントローラー）
        if (rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool aPressed) && aPressed)
        {
            if (!isRequesting)
            {
                StartCoroutine(TryLogin(userIDField.text, passwordField.text));
            }
        }

        // PCデバッグ用
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (!isRequesting)
            {
                StartCoroutine(TryLogin(userIDField.text, passwordField.text));
            }
        }
    }

    // XR右コントローラー取得
    private void GetRightController()
    {
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller,
            devices);

        if (devices.Count > 0)
        {
            rightController = devices[0];
        }
    }

    IEnumerator TryLogin(string username, string password)
    {
        isRequesting = true;

        LoginRequest req = new LoginRequest(username, password);
        string json = JsonUtility.ToJson(req);

        yield return StartCoroutine(api.Post("/login", json, (response) =>
        {
            Debug.Log("Raw Login Response: " + response);

            LoginResponse res = JsonUtility.FromJson<LoginResponse>(response);

            if (res.message == "Login successful")
            {
                Debug.Log("ログイン成功！");
                Debug.Log("ユーザー名: " + res.user_name);

                PlayerMode.SetPlayerName(username);
                GoToPlayScene();
            }
            else
            {
                Debug.LogError("ログイン失敗: " + res.message);

                // 入力欄リセット
                userIDField.text = "";
                passwordField.text = "";
            }
        }));

        isRequesting = false;
    }

    public void LoginStart()
    {
        isLogin = true;
        loginCanvas.enabled = true;
    }

    private void GoToPlayScene()
    {
        SceneManager.LoadScene("PlayScene");
        Debug.Log("Go to Play Scene");
    }

    // JSON用クラス
    [System.Serializable]
    public class LoginRequest
    {
        public string username;
        public string password;

        public LoginRequest(string user, string pass)
        {
            username = user;
            password = pass;
        }
    }

    [System.Serializable]
    public class LoginResponse
    {
        public string message;
        public string user_name;
    }
}
