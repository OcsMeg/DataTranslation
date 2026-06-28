using UnityEngine;

public class SpawnSphereOnFist : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private HandGestureDetector gestureDetector;
    [SerializeField] private GameObject spherePrefab;

    [Header("生成オフセット")]
    [Tooltip("手のひらからどれだけ前方にずらして生成するか")]
    public float offsetForward = 0.05f;

    private void Reset()
    {
        // 同じ GameObject に HandGestureDetector がある場合は自動取得
        if (gestureDetector == null)
            gestureDetector = GetComponent<HandGestureDetector>();
    }

    private void OnEnable()
    {
        if (gestureDetector != null)
        {
            gestureDetector.OnFist += HandleFist;
        }
    }

    private void OnDisable()
    {
        if (gestureDetector != null)
        {
            gestureDetector.OnFist -= HandleFist;
        }
    }

    private void HandleFist()
    {
        if (spherePrefab == null)
        {
            Debug.LogWarning("SpawnSphereOnFist: spherePrefab が設定されていません。");
            return;
        }

        if (gestureDetector == null || !gestureDetector.IsHandTracked)
        {
            Debug.LogWarning("SpawnSphereOnFist: 手がトラッキングされていません。");
            return;
        }

        // 手のひら位置＋前方少しオフセット
        Pose palm = gestureDetector.PalmPose;
        Vector3 spawnPos = palm.position + palm.forward * offsetForward;

        Instantiate(spherePrefab, spawnPos, Quaternion.identity);
    }
}