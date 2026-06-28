using UnityEngine;

public class HideMRCharacterLayer : MonoBehaviour
{
    void Start()
    {
        // 非表示にしたいレイヤーの名前
        string layerName = "MRCharacter";

        // レイヤー名からレイヤーIDを取得します
        int layerID = LayerMask.NameToLayer(layerName);

        // レイヤーが存在するか確認します
        if (layerID == -1)
        {
            Debug.LogWarning("レイヤー '" + layerName + "' が存在しません。Project Settings > Tags and Layers で定義してください。");
            return;
        }

        // メインカメラを取得します
        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            // カメラのカリングマスクから 'MRCharacter' レイヤーを除外します
            
            // (1 << layerID) で 'MRCharacter' レイヤーのみのマスクを作成
            int layerMask = 1 << layerID;

            // ~ (ビット反転) で 'MRCharacter' 以外すべてのレイヤーのマスクを作成
            // & (ビットAND) で現在のカリングマスクと組み合わせ、指定したレイヤーだけをオフにします
            mainCamera.cullingMask &= ~layerMask;
            
            Debug.Log("'" + layerName + "' レイヤーをメインカメラの表示から除外しました。");
        }
        else
        {
            Debug.LogError("メインカメラが見つかりません。カメラに 'MainCamera' タグが設定されているか確認してください。");
        }
    }
}