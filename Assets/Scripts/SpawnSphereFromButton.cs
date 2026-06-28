using UnityEngine;

public class SpawnSphereFromButton : MonoBehaviour
{
    [Header("生成する球のプレハブ")]
    [SerializeField] private GameObject spherePrefab;

    [Header("生成の基準位置（普通は PhotoDisplay の root）")]
    [SerializeField] private Transform spawnOrigin;

    [Header("パネルから前方への距離")]
    [SerializeField] private float distanceFromPanel = 0.2f;

    /// <summary>
    /// UI Button の OnClick から呼び出す
    /// </summary>
    public void Spawn()
    {
        if (spherePrefab == null || spawnOrigin == null)
        {
            Debug.LogWarning("SpawnSphereFromButton: Prefab か SpawnOrigin が設定されていません。");
            return;
        }

        Vector3 pos = spawnOrigin.position + spawnOrigin.forward * distanceFromPanel;
        Quaternion rot = Quaternion.identity;

        Instantiate(spherePrefab, pos, rot);
    }
}