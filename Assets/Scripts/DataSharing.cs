using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class DataSharing : MonoBehaviour
{
    private List<int> sharingFileIDs;
    private ApiClient api;
    private string currentUser; //共有するユーザのID
    private int fileIdField; //共有したいファイルID
    private string targetID; //共有したい相手のID
    private GameObject shareCard;
    private bool isSharing = false;
    private ImageLoader imageLoader;
    private PhotonView photonView;
    private HoverMethod hoverMethod;
    [SerializeField] private GUIMethod guiMethod;
    [SerializeField] private GUIShareButton guiShareButton;
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
        currentUser = PlayerMode.GetPlayerName();
        imageLoader = GetComponent<ImageLoader>();
        photonView = GetComponent<PhotonView>();
        hoverMethod = GetComponent<HoverMethod>();
        guiMethod = GetComponent<GUIMethod>();
        //guiShareButton = GetComponentInChildren<GUIShareButton>();
    }
    public void StartSharing(List<int> selectedID)
    {
        if (!photonView.IsMine) return;
        sharingFileIDs = selectedID;
        // 共有手法ごとに処理を分ける
        // ホバー型（かざし型）
        if (ShareMode.GetShareMethod() == ShareMode.ShareMethod.Hover)
        {
            hoverMethod.InstantiateSharingCard(sharingFileIDs);
            imageLoader.CloseDisplayCanvas();
        } // Ray型
        else if (ShareMode.GetShareMethod() == ShareMode.ShareMethod.Ray)
        {
            imageLoader.CloseDisplayCanvas();
        } // GUI操作型
        else if (ShareMode.GetShareMethod() == ShareMode.ShareMethod.GUI)
        {
            guiMethod.SelectRecipient();
            //guiShareButton.SetSearchToggle();
        }
        isSharing = true;
    }
    
    public void ReceiveUserID(string userID)
    {
        if (isSharing)
        {
            targetID = userID;
            foreach (var f in sharingFileIDs)
            {
                StartCoroutine(ShareFile(f));
            }
            
            // 交換が終わったらshareCardを消す
            //shareCard.SetActive(false);
        }
    }
    
    IEnumerator ShareFile(int fileID)
    {
        // 入力値読み取り
        string targetUser = targetID;

        Debug.Log($"[Share] file={fileID}, to={targetUser}, by={currentUser}");

        // JSON 作成
        ShareRequest req = new ShareRequest(fileID, currentUser, targetUser);
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
    }
}
