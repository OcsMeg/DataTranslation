using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CanvasEventCameraSetter : MonoBehaviour
{
    private IEnumerator Start()
    {
        // MainCamera が生成されるまで待つ（プレイヤー生成より後にしたい場合用）
        while (Camera.main == null)
        {
            yield return null;
        }

        var canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.worldCamera = Camera.main;
            Debug.Log($"[CanvasEventCameraSetter] Event Camera を {Camera.main.name} にセットしました");
        }
    }
}