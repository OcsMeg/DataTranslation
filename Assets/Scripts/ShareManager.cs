using System.Collections;
using UnityEngine;
using TMPro;

public class ShareManager : MonoBehaviour
{
    private ApiClient api;

    [SerializeField] private int fileIdField;     // 共有したいファイルID
    [SerializeField] private string shareWithField;  // 共有したい相手のユーザ名

    // ログイン中ユーザ名（LoginManagerでセットした想定）
    [SerializeField] private string currentUser;

    private bool isTriggered = false;

    [System.Serializable]
    public class ShareRequest
    {
        public int file_id;
        public string shared_by;
        public string shared_with;

        public ShareRequest(int fileId, string by, string withUser)
        {
            file_id = fileId;
            shared_by = by;
            shared_with = withUser;
        }
    }

    [System.Serializable]
    public class ShareResponse
    {
        public string message;
    }

    void Start()
    {
        api = GetComponent<ApiClient>();
        if (api == null)
            Debug.LogError("ApiClient が見つかりません");

        // ログイン時に PlayerMode などで保存してある前提
        //currentUser = PlayerMode.GetPlayerName();
        //Debug.Log("[ShareManager] currentUser = " + currentUser);
    }

    void Update()
    {
        // Quest3 → Aボタン, PC → Sキー
        if ((OVRInput.GetDown(OVRInput.Button.One) || Input.GetKeyDown(KeyCode.S)) && !isTriggered)
        {
            isTriggered = true;
            StartCoroutine(ShareFile());
        }
    }

    IEnumerator ShareFile()
    {
        // 入力値読み取り
        int fileId = fileIdField;
        string targetUser = shareWithField;

        Debug.Log($"[Share] file={fileId}, to={targetUser}, by={currentUser}");

        // JSON 作成
        ShareRequest req = new ShareRequest(fileId, currentUser, targetUser);
        string json = JsonUtility.ToJson(req);

        // POST 実行
        yield return StartCoroutine(api.Post("/share", json, (response) =>
        {
            Debug.Log("[Share Response Raw] " + response);

            ShareResponse res = JsonUtility.FromJson<ShareResponse>(response);

            if (res.message != null)
            {
                Debug.Log("[Share] 成功: " + res.message);
            }
            else
            {
                Debug.LogError("[Share] 失敗: " + response);
            }
        }));

        isTriggered = false;
    }
}

