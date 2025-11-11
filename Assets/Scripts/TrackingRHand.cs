using UnityEngine;

public class TrackingRHand : MonoBehaviour
{
    private GameObject rHandTargetAnchor;

    void Start()
    {
        
    }

    void Update()
    {
        // 見つかっている場合は、このオブジェクトの位置を合わせる
        if (rHandTargetAnchor != null)
        {
            transform.position = rHandTargetAnchor.transform.position;
        }
    }
    
    private void FindRHandTarget(GameObject card)
    {
        // シーン内から "RHandTargetAnchor" という名前のオブジェクトを探す
        rHandTargetAnchor = card;

        if (rHandTargetAnchor == null)
        {
            Debug.LogWarning("RHandTargetAnchor が見つかりませんでした。名前を確認してください。");
        }
    }

    public void TrackCard(GameObject card)
    {
        FindRHandTarget(card);
    }
}
