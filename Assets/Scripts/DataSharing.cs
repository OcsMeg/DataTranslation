using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class DataSharing : MonoBehaviour
{
    private List<int> sharingFileIDs;
    private ApiClient api;
    private string currentUser;
    private int fileIdField;
    private string targetID;
    private GameObject shareCard;

    private bool isSharing = false;

    // 外部（CardManager等）から参照できる読み取り専用プロパティ
    public bool IsSharing => isSharing;

    // 複数ファイル共有時に、全部終わるまで isSharing=true を維持する
    private int pendingShareCount = 0;

    private ImageLoader imageLoader;
    private PhotonView photonView;
    private HoverMethod hoverMethod;
    [SerializeField] private GUIMethod guiMethod;
    [SerializeField] private GUIShareButton guiShareButton;
    private ControllerRaySelect raySelect;
    [SerializeField] private TextMeshProUGUI debugText;


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
        raySelect = GetComponentInChildren<ControllerRaySelect>();
    }

    public void StartSharing(List<int> selectedID)
    {
        if (!photonView.IsMine) return;

        if (isSharing) return;
        sharingFileIDs = selectedID;

        // 共有開始時点で「共有中」にする
        pendingShareCount = (sharingFileIDs != null) ? sharingFileIDs.Count : 0;
        isSharing = true;

        // 共有手法ごとに処理を分ける
        if (ShareMode.GetShareMethod() == ShareMode.ShareMethod.Hover)
        {
            hoverMethod.InstantiateSharingCard(sharingFileIDs);
            imageLoader.CloseDisplayCanvas();
        }
        else if (ShareMode.GetShareMethod() == ShareMode.ShareMethod.Ray)
        {
            raySelect.ActivateRay();
            imageLoader.CloseDisplayCanvas();
        }
        else if (ShareMode.GetShareMethod() == ShareMode.ShareMethod.GUI)
        {
            guiMethod.SelectRecipient();
        }

        //isSharing = false;
        // もし選択が空なら即解除（無限に共有中にならないように）
        // if (pendingShareCount <= 0)
        // {
        //     isSharing = false;
        // }
    }

    public void ReceiveUserID(string userID)
    {
        if (!isSharing) return;

        targetID = userID;

        foreach (var f in sharingFileIDs)
        {
            StartCoroutine(ShareFile(f));
        }
    }

    IEnumerator ShareFile(int fileID)
    {
        string targetUser = targetID;
        Debug.Log($"[Share] file={fileID}, to={targetUser}, by={currentUser}");

        ShareRequest req = new ShareRequest(fileID, currentUser, targetUser);
        string json = JsonUtility.ToJson(req);

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

            // 変更：1ファイル終わるたびに減らして、全部終わったら共有終了
            pendingShareCount--;
            if (pendingShareCount <= 0)
            {
                if (ShareMode.GetShareMethod() == ShareMode.ShareMethod.Hover)
                {
                    hoverMethod.DestroySharingCard();
                    StopSharing();
                } else if (ShareMode.GetShareMethod() == ShareMode.ShareMethod.Ray)
                {
                    raySelect.DeactivateRay();
                } else if (ShareMode.GetShareMethod() == ShareMode.ShareMethod.GUI)
                {
                    guiMethod.CancelShare();
                }
            }
        }));
    }

    public void StopSharing()
    {
        isSharing = false;
    }

    void Update()
    {
        if (isSharing)
        {
            debugText.text = "true";
        }
        else
        {
            debugText.text = "false";
        }
    }
}
