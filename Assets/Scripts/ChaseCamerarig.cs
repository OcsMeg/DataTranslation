using UnityEngine;

public class ChaseCamerarig : MonoBehaviour
{
    [SerializeField] private GameObject rootObject; // ここ配下から Model を探す（PlayerRootなど）
    [SerializeField] private string modelTag = "Model";

    private Transform model;

    // void Start()
    // {
    //     model = FindModel(rootObject);
    // }

    void LateUpdate()
    {
        transform.position = model.position;
        transform.rotation = model.rotation;
    }

    /// <summary>
    /// 引数で渡した GameObject の子階層から、Modelタグが付いた GameObject(Transform) を探して返す
    /// </summary>
    public void FindModel(GameObject root)
    {
        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
        {
            if (t != null && t.CompareTag(modelTag))
            {
                model = t;
            }
        }
    }
}