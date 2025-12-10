using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandUIRaycaster : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private HandGestureDetector rightHandDetector;
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private Camera uiCamera;

    [Header("押し判定")]
    [Tooltip("指先とUIヒット位置の距離がこの値より小さいときに『押している』とみなす（m）")]
    [SerializeField] private float pressDistance = 0.01f;

    [Tooltip("この距離より離れたら Hover をやめる（m）。pressDistance 以上にする")]
    [SerializeField] private float hoverDistance = 0.03f;

    private GameObject currentHover = null;
    private GameObject currentPressed = null;
    private bool hasClickedOnCurrentHover = false;

    public bool IsPressingUI => currentPressed != null;

    void OnValidate()
    {
        // うっかり押し距離 > ホバー距離にしないように補正
        if (pressDistance > hoverDistance)
            pressDistance = hoverDistance;
    }

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

            // ★ まず Z 距離を見る
            float dist = Vector3.Distance(tipPose.position, hit.worldPosition);

            // hoverDistance より遠いときは「何も指していない」扱いにする
            if (dist < hoverDistance)
            {
                newHover = ExecuteEvents.GetEventHandler<IPointerEnterHandler>(hit.gameObject);
            }
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
            hasClickedOnCurrentHover = false;

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

            hasClickedOnCurrentHover = false;
            return;
        }

        // ======== 押し込み距離で press / click 判定 ========
        float pressDist = Vector3.Distance(tipPose.position, hit.worldPosition);
        bool insidePress = pressDist < pressDistance;

        if (insidePress && currentPressed == null)
        {
            currentPressed = currentHover;

            ExecuteEvents.ExecuteHierarchy(
                currentPressed,
                pointerData,
                ExecuteEvents.pointerDownHandler
            );
        }
        else if (!insidePress && currentPressed != null)
        {
            ExecuteEvents.ExecuteHierarchy(
                currentPressed,
                pointerData,
                ExecuteEvents.pointerUpHandler
            );

            if (!hasClickedOnCurrentHover && currentPressed == currentHover)
            {
                ExecuteEvents.ExecuteHierarchy(
                    currentPressed,
                    pointerData,
                    ExecuteEvents.pointerClickHandler
                );
                hasClickedOnCurrentHover = true;
            }

            currentPressed = null;
        }
    }
}
