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
        GameObject playerRoot = GameObject.FindGameObjectWithTag("PlayerRoot");
        dataSharing = playerRoot.GetComponent<DataSharing>();
    }

    public string GetUserID()
    {
        return userID;
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = info.photonView.InstantiationData;

        if (data != null && data.Length > 0)
        {
            userID = data[0] as string;
            Debug.Log($"生成したユーザーID: {userID}");
        }
        else
        {
            Debug.Log("データがないです");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        var otherView = other.GetComponent<PhotonView>();
        if (otherView != null && otherView.IsMine)
        {
            Debug.Log($"このオブジェクトは {userID}が出したものです");
            dataSharing.ReceiveUserID(userID);
        }
    }
}
