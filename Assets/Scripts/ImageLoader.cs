using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.XR;
using System.Collections.Generic;

public class ImageLoader : MonoBehaviour
{
    private string userName;
    [SerializeField] private Transform parent;
    [SerializeField] private GameObject imagePrefab;
    [SerializeField] private Canvas displayCanvas;

    private ApiClient api;
    private bool isTriggered = false;
    private string fileName;
    private int fileNum;
    private PhotonView photonView;
    private bool prevXPressed = false;
    // XR 左コントローラー
    private InputDevice leftController;
    private bool closeCanvas = false;

    // ---------- APIレスポンス用データクラス ----------
    [System.Serializable]
    public class FileInfo
    {
        public int file_id;
        public string file_name;
        public string s3_path;
        public string data_type;
        public string owner;
    }

    [System.Serializable]
    public class FilesResponse
    {
        public string user;
        public FileInfo[] files;
    }

    [System.Serializable]
    public class DownloadInfo
    {
        public string file_name;
        public string download_url;
    }
    // ---------------------------------------------------

    void Start()
    {
        api = GetComponent<ApiClient>();
        photonView = GetComponent<PhotonView>();

        userName = PlayerMode.GetPlayerName();

        displayCanvas.enabled = false;

        GetLeftController();
    }

    void Update()
    {
        if (!leftController.isValid)
            GetLeftController();

        if (!photonView.IsMine) return;

        bool xPressedNow = false;

        // XRコントローラーの Xボタン状態を取得
        if (leftController.TryGetFeatureValue(CommonUsages.primaryButton, out bool xPressed))
            xPressedNow = xPressed;

        bool xButtonDown = (xPressedNow && !prevXPressed); // ← 1回押しのみ検知！

        // キーボード P も同様に扱う
        bool pKeyDown = Input.GetKeyDown(KeyCode.P);

        // トグル条件をまとめる
        bool togglePressed = xButtonDown || pKeyDown || closeCanvas;


        if (togglePressed)
        {
            // 表示状態をトグル
            displayCanvas.enabled = !displayCanvas.enabled;

            // 初回表示なら画像読み込み
            if (displayCanvas.enabled && !isTriggered)
            {
                isTriggered = true;
                StartCoroutine(LoadImagesFlow());
            }
        }

        // 前回の状態を保存
        prevXPressed = xPressedNow;
        // closeCanvasはリセット
        closeCanvas = false;
    }

    public void CloseDisplayCanvas()
    {
        closeCanvas = true;
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
            Debug.Log("左コントローラー取得 (ImageLoader)");
        }
    }

    IEnumerator LoadImagesFlow()
    {
        Debug.Log("=== STEP1: /files を取得 ===");

        yield return StartCoroutine(api.Get($"/files?user_name={userName}", (json) =>
        {
            Debug.Log("[/files] Response JSON: " + json);

            FilesResponse res = JsonUtility.FromJson<FilesResponse>(json);

            if (res.files == null || res.files.Length == 0)
            {
                Debug.LogWarning("ファイルが1件もありません");
                return;
            }

            foreach (var f in res.files)
            {
                fileName = f.file_name;
                fileNum = f.file_id;
                StartCoroutine(LoadOneFile(f.file_id));
            }
        }));
    }

    IEnumerator LoadOneFile(int id)
    {
        Debug.Log($"=== STEP2: /files/{id} を取得 ===");

        yield return StartCoroutine(api.Get($"/files/{id}", (json) =>
        {
            Debug.Log($"[/files/{id}] Response JSON: " + json);

            DownloadInfo dl = JsonUtility.FromJson<DownloadInfo>(json);

            StartCoroutine(DownloadImage(dl.download_url));
        }));
    }

    IEnumerator DownloadImage(string url)
    {
        Debug.Log("[ImageLoader] 画像取得: " + url);

        UnityWebRequest req = UnityWebRequestTexture.GetTexture(url);
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("[ImageLoader] DL Error: " + req.error);
            yield break;
        }

        Texture2D tex = DownloadHandlerTexture.GetContent(req);

        // Prefab 作成
        GameObject obj = Instantiate(imagePrefab, parent);
        RawImage img = obj.GetComponent<RawImage>();
        img.texture = tex;

        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(300, 300);

        TextMeshProUGUI tmp = obj.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null) tmp.text = fileName;
        
        FileIDMemory mem = obj.GetComponent<FileIDMemory>();
        mem.SetFileID(fileNum);

        Debug.Log("[ImageLoader] 画像生成完了");
        
        //トグルをセットする
        ShareButton shareButtonScript = GetComponentInChildren<ShareButton>();
        shareButtonScript.SetSearchToggle();
    }
}
