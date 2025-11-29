using UnityEngine;
using UnityEngine.Events;

public class HandTouchButton : MonoBehaviour
{
    [Header("右手のジェスチャ検出")]
    [SerializeField] private HandGestureDetector rightHandDetector;   // useRightHand = true の方

    [Header("判定設定")]
    [Tooltip("指先とボタン中心の距離がこの値より小さくなった瞬間に押下と判定")]
    [SerializeField] private float pressDistance = 0.02f;

    [Header("押されたときに呼ぶイベント")]
    public UnityEvent onPressed;

    private bool wasInside = false;

    private void Update()
    {
        if (rightHandDetector == null || !rightHandDetector.IsHandTracked)
        {
            wasInside = false;
            return;
        }

        Pose tipPose = rightHandDetector.IndexTipPose;

        // ボタン（RectTransform）の中心
        Vector3 buttonPos = transform.position;

        float dist = Vector3.Distance(tipPose.position, buttonPos);
        bool isInside = dist <= pressDistance;

        // 触れた「瞬間」だけ onPressed を呼ぶ
        if (isInside && !wasInside)
        {
            Debug.Log("HandTouchButton pressed!");
            onPressed?.Invoke();
        }

        wasInside = isInside;
    }
}