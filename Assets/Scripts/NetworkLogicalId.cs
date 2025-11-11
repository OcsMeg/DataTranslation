using UnityEngine;
using Photon.Pun;

public class NetworkLogicalId : MonoBehaviour, IPunInstantiateMagicCallback
{
    // Inspector で見たいので SerializeField
    [SerializeField] private string logicalIdForDebug;

    // 外から取得したいとき用のプロパティ
    public string LogicalId => logicalIdForDebug;

    /// <summary>
    /// 他のスクリプトから呼ぶ用のメソッド
    /// </summary>
    public string GetLogicalId()
    {
        return logicalIdForDebug;
    }

    // Photon で Instantiate されたときに自動で呼ばれる
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = info.photonView.InstantiationData;

        if (data != null && data.Length > 0)
        {
            logicalIdForDebug = data[0] as string;
            Debug.Log($"[NetworkLogicalId] 受け取った論理ID: {logicalIdForDebug}");
        }
        else
        {
            Debug.LogWarning("[NetworkLogicalId] InstantiationData が空です");
        }
    }
}