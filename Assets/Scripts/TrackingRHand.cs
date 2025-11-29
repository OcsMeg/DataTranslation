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
            transform.rotation = rHandTargetAnchor.transform.rotation;
        }
    }
    
    private void FindRHandTarget(GameObject rTarget)
    {
        // シーン内から "RHandTargetAnchor" という名前のオブジェクトを探す
        rHandTargetAnchor = rTarget;

        if (rHandTargetAnchor == null)
        {
            Debug.LogWarning("RHandTargetAnchor が見つかりませんでした。名前を確認してください。");
        }
    }

    public void TrackCard(GameObject rTarget)
    {
        FindRHandTarget(rTarget);
    }
}
