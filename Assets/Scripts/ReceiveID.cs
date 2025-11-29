using System;
using UnityEngine;
using Photon.Pun;

public class ReceiveID : MonoBehaviour, IPunInstantiateMagicCallback
{
    [SerializeField] private string userID;
    private PhotonView photonView;
    private DataSharing dataSharing;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

        // ローカルプレイヤーの DataSharing を探す
        foreach (var ds in FindObjectsOfType<DataSharing>())
        {
            var pv = ds.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)
            {
                dataSharing = ds;
                break;
            }
        }

        if (dataSharing == null)
        {
            Debug.LogError("ReceiveID: ローカルの DataSharing が見つかりません");
        }
    }

    public string GetUserID()
    {
        return userID;
    }

    // ReceivingCard 生成時に一度だけ呼ばれる
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = info.photonView.InstantiationData;

        if (data != null && data.Length > 0)
        {
            // CardManager から new object[]{ userID } で渡されたもの
            userID = data[0].ToString();
            Debug.Log($"[ReceiveID] 生成したユーザーID: {userID}");
        }
        else
        {
            Debug.LogWarning("[ReceiveID] InstantiationData がありません");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var otherView = other.GetComponent<PhotonView>();
        if (otherView != null && otherView.IsMine)
        {
            Debug.Log($"[ReceiveID] このReceivingCardの持ち主は {userID} です");

            if (dataSharing != null)
            {
                dataSharing.ReceiveUserID(userID);
            }
            else
            {
                Debug.LogError("[ReceiveID] dataSharing が設定されていません");
            }
        }
    }
}