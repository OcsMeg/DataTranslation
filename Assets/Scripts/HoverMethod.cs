using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HoverMethod : MonoBehaviour
{
    [SerializeField] private GameObject RHandTarget;
    private GameObject shareCard;
    
    void Start()
    {
        
    }
    
    public void InstantiateSharingCard(List<int> sharingFileIDs)
    {
        shareCard = PhotonNetwork.Instantiate("SendingCardRoot", RHandTarget.transform.position, RHandTarget.transform.rotation, 0,new object[] { sharingFileIDs[0] });
        // 追従スクリプトにターゲットを渡す
        TrackingRHand trackingRHand = shareCard.GetComponentInChildren<TrackingRHand>();
        trackingRHand.TrackCard(RHandTarget);
    }

    public void DestroySharingCard()
    {
        PhotonNetwork.Destroy(shareCard);
    }
}
