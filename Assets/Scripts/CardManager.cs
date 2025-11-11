using UnityEngine;
using System.Collections;
using Photon.Pun;

public class CardManager : MonoBehaviour
{
    [SerializeField] private GameObject RHandTarget;
    
    // Yボタンが押された状態かを管理
    private bool isYButtonPressed = false;
    
    // カードがあるかどうか
    private bool isCard = false;
    
    // クールダウン時間（秒）
    [SerializeField] private float cooldownTime = 0.5f;
    
    //UserID
    public string userID = "User_1234";
    
    private GameObject card;

    void Start()
    {

    }

    void Update()
    {
        // Yボタンの入力を検出
        bool yButtonDown = OVRInput.Get(OVRInput.Button.Two);
        
        // Yボタンが押されていて、クールダウン中でない場合
        if (yButtonDown && !isCard)
        {
            
            // カードの表示/非表示を切り替え
            CardInstatiate();
        }
    }

    private void CardInstatiate()
    {
        card = PhotonNetwork.Instantiate("PreCard", RHandTarget.transform.position, Quaternion.identity, 0, new object[] { userID });
        TrackingRHand trackingRHand = card.GetComponent<TrackingRHand>();
        trackingRHand.TrackCard(RHandTarget);
        isCard = true;
    }
}