using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ImageLoader : MonoBehaviour
{
    private string userName;

    [SerializeField] private Transform parent;
    [SerializeField] private GameObject imagePrefab;
    [SerializeField] private Canvas displayCanvas;

    private ApiClient api;
    private string fileName;
    private int fileNum;
    private PhotonView photonView;
    [SerializeField] private GameObject CanvasSpawn;
    private int pending;

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

    /// <summary>現在 Canvas が表示中かどうか</summary>
    public bool IsDisplayVisible => displayCanvas != null && displayCanvas.enabled;

    void Start()
    {
        api = GetComponent<ApiClient>();
        photonView = GetComponent<PhotonView>();
        userName = PlayerMode.GetPlayerName();

        if (displayCanvas != null)
            displayCanvas.enabled = false;
    }

    #region 外部から呼ぶ用 API

    /// <summary>
    /// 表示・非表示をトグル
    /// </summary>
    public void ToggleDisplay()
    {
        if (IsDisplayVisible)
        {
            HideDisplay();
        }
        else
        {
            ShowDisplay();
        }
    }

    /// <summary>
    /// Canvas を表示し、画像リストを読み込む
    /// </summary>
    private void ShowDisplay()
    {
        if (displayCanvas == null) return;

        displayCanvas.enabled = true;
        displayCanvas.transform.position = CanvasSpawn.transform.position;
        displayCanvas.transform.rotation = CanvasSpawn.transform.rotation;
        ClearImages();
        StartCoroutine(LoadImagesFlow());
    }

    /// <summary>
    /// Canvas を非表示にし、画像をクリア
    /// </summary>
    private void HideDisplay()
    {
        if (displayCanvas == null) return;

        displayCanvas.enabled = false;
        ClearImages();
    }

    /// <summary>
    /// UI ボタンから呼ぶ用（既存の CloseDisplayCanvas を生かす）
    /// </summary>
    public void CloseDisplayCanvas()
    {
        HideDisplay();
    }

    #endregion

    /// <summary>
    /// ImagePrefab の全削除 or 非表示
    /// </summary>
    private void ClearImages()
    {
        if (parent == null) return;

        foreach (Transform child in parent)
        {
            child.gameObject.SetActive(false); // 完全に消したいなら Destroy(child.gameObject);
        }
    }

    IEnumerator LoadImagesFlow()
    {
        yield return StartCoroutine(api.Get($"/files?user_name={userName}", (json) =>
        {
            FilesResponse res = JsonUtility.FromJson<FilesResponse>(json);

            if (res.files == null || res.files.Length == 0)
            {
                Debug.LogWarning("ファイルが1件もありません");
                return;
            }

            pending = res.files.Length;

            foreach (var f in res.files)
            {
                int id = f.file_id;          // ★ローカルに保持
                string name = f.file_name;   // ★ローカルに保持
                StartCoroutine(LoadOneFile(id, name));
            }
        }));
    }

    IEnumerator LoadOneFile(int id, string name)
    {
        yield return StartCoroutine(api.Get($"/files/{id}", (json) =>
        {
            DownloadInfo dl = JsonUtility.FromJson<DownloadInfo>(json);
            StartCoroutine(DownloadImage(dl.download_url, id, name));
        }));
    }

    IEnumerator DownloadImage(string url, int id, string name)
    {
        UnityWebRequest req = UnityWebRequestTexture.GetTexture(url);
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("[ImageLoader] DL Error: " + req.error);
            yield break;
        }

        Texture2D tex = DownloadHandlerTexture.GetContent(req);

        GameObject obj = Instantiate(imagePrefab, parent);

        RawImage img = obj.GetComponentInChildren<RawImage>();
        img.texture = tex;

        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(300, 300);

        TextMeshProUGUI tmp = obj.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null) tmp.text = name;      // ここで name

        FileIDMemory mem = obj.GetComponent<FileIDMemory>();
        mem.SetFileID(id);                     // ここで id

        // （任意）最後の1枚が生成されたタイミングで1回だけ
        pending--;
        if (pending == 0)
        {
            ShareButton shareButtonScript = GetComponentInChildren<ShareButton>();
            shareButtonScript.SetSearchToggle();
        }
    }
}
