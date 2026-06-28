using UnityEngine;

/// <summary>
/// 左手のジェスチャに応じて PhotoDisplay(Canvas) を表示・追従させる
/// </summary>
public class PhotoDisplayController : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private HandGestureDetector leftHandDetector; // useRightHand=false の方
    [SerializeField] private Canvas displayCanvas;                 // PhotoDisplay の Canvas

    [Header("手のひらからのオフセット")]
    [Tooltip("手のひらローカル座標系での位置オフセット")]
    [SerializeField] private Vector3 positionOffset = new Vector3(0.0f, 0.05f, 0.15f);

    [Tooltip("手のひらの回転に足すオイラー角（度）")]
    [SerializeField] private Vector3 rotationOffsetEuler = new Vector3(0, 180f, 0);

    private Quaternion rotationOffset;
    private bool isVisible = false;

    private Transform DisplayTransform => displayCanvas != null ? displayCanvas.transform : null;

    private void Awake()
    {
        rotationOffset = Quaternion.Euler(rotationOffsetEuler);
    }

    private void Start()
    {
        // 最初は非表示
        if (displayCanvas != null)
        {
            displayCanvas.enabled = false;
        }
    }

    private void OnEnable()
    {
        if (leftHandDetector != null)
        {
            leftHandDetector.OnFist += OnLeftHandFist;
        }
    }

    private void OnDisable()
    {
        if (leftHandDetector != null)
        {
            leftHandDetector.OnFist -= OnLeftHandFist;
        }
    }

    /// <summary>
    /// 左手がグーになった瞬間に呼ばれる（表示ON/OFFトグル）
    /// </summary>
    private void OnLeftHandFist()
    {
        isVisible = !isVisible;

        if (displayCanvas != null)
        {
            displayCanvas.enabled = isVisible;
        }
    }

    private void Update()
    {
        if (!isVisible || leftHandDetector == null || !leftHandDetector.IsHandTracked)
            return;

        if (DisplayTransform == null)
            return;

        Pose palm = leftHandDetector.PalmPose;

        // 手のひらからの相対位置
        Vector3 targetPos = palm.position + palm.rotation * positionOffset;

        // ★向きを 180 度反転させる（Y軸回り）
        Quaternion targetRot =
            palm.rotation *
            rotationOffset *
            Quaternion.Euler(0f, 180f, 0f);

        DisplayTransform.SetPositionAndRotation(targetPos, targetRot);
    }
}
