using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandUIRaycaster : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private HandGestureDetector rightHandDetector; // 右手用（Use Right Hand = true）
    [SerializeField] private GraphicRaycaster graphicRaycaster;     // 画像＆ボタンのある Canvas の Raycaster
    [SerializeField] private EventSystem eventSystem;               // シーン内の EventSystem
    [SerializeField] private Camera uiCamera;                       // Canvas の Event Camera（通常 MainCamera）

    [Header("押し判定")]
    [Tooltip("指先とUIヒット位置の距離がこの値より小さいときに『押している』とみなす（m）")]
    [SerializeField] private float pressDistance = 0.01f; // 1cm くらい

    private GameObject currentHover = null;   // いま指が乗っている UI
    private GameObject currentPressed = null; // 押し込み中の UI

    void Awake()
    {
        if (uiCamera == null)
            uiCamera = Camera.main;

        if (eventSystem == null)
            eventSystem = FindAnyObjectByType<EventSystem>();
    }

    void Update()
    {
        if (rightHandDetector == null || !rightHandDetector.IsHandTracked)
            return;

        if (graphicRaycaster == null || eventSystem == null || uiCamera == null)
            return;

        Pose tipPose = rightHandDetector.IndexTipPose;

        // 指先のワールド座標 → スクリーン座標
        Vector3 screenPos = uiCamera.WorldToScreenPoint(tipPose.position);
        var pointerData = new PointerEventData(eventSystem)
        {
            position = screenPos
        };

        // UI への Raycast
        var results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerData, results);

        GameObject newHover = null;
        RaycastResult hit = default;

        if (results.Count > 0)
        {
            hit = results[0];

            // Button / Toggle など「クリックできるオブジェクト」を探す
            newHover = ExecuteEvents.GetEventHandler<IPointerEnterHandler>(hit.gameObject);
        }

        // ======== Hover の更新（Enter / Exit） ========
        if (currentHover != newHover)
        {
            if (currentHover != null)
            {
                ExecuteEvents.ExecuteHierarchy(
                    currentHover,
                    pointerData,
                    ExecuteEvents.pointerExitHandler
                );
            }

            currentHover = newHover;

            if (currentHover != null)
            {
                ExecuteEvents.ExecuteHierarchy(
                    currentHover,
                    pointerData,
                    ExecuteEvents.pointerEnterHandler
                );
            }
        }

        // 何も UI を指していないなら、押し状態をリセット
        if (currentHover == null)
        {
            if (currentPressed != null)
            {
                ExecuteEvents.ExecuteHierarchy(
                    currentPressed,
                    pointerData,
                    ExecuteEvents.pointerUpHandler
                );
                currentPressed = null;
            }
            return;
        }

        // ======== 押し込み距離で press 判定 ========
        float dist = Vector3.Distance(tipPose.position, hit.worldPosition);
        bool insidePress = dist < pressDistance;

        // まだ押していない状態で、押し込み距離内に入った → pointerDown
        if (insidePress && currentPressed == null)
        {
            currentPressed = currentHover;

            ExecuteEvents.ExecuteHierarchy(
                currentPressed,
                pointerData,
                ExecuteEvents.pointerDownHandler
            );
        }
        // 押している状態で、押し込み距離外に出た → pointerUp (+ クリック)
        else if (!insidePress && currentPressed != null)
        {
            // Up
            ExecuteEvents.ExecuteHierarchy(
                currentPressed,
                pointerData,
                ExecuteEvents.pointerUpHandler
            );

            // 離した位置も同じ UI 上なら Click 扱い
            if (currentPressed == currentHover)
            {
                ExecuteEvents.ExecuteHierarchy(
                    currentPressed,
                    pointerData,
                    ExecuteEvents.pointerClickHandler
                );
            }

            currentPressed = null;
        }
    }
}
