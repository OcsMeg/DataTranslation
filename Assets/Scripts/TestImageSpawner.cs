using UnityEngine;
using UnityEngine.UI; // RawImage を使うために必要
using OVR;
using TMPro; // Meta Quest (Oculus Integration) SDK が必要

/// <summary>
/// Meta QuestのトリガーまたはキーボードのOキーで、
/// 指定した画像をPrefabに設定し、指定した場所にインスタンス化するスクリプト。
///
/// 注意：このスクリプトは Oculus Integration SDK (OVR) が
/// プロジェクトにインポートされている必要があります。
/// </summary>
public class TestImageSpawner : MonoBehaviour
{
    [Header("設定")]
    [Tooltip("RawImageコンポーネントを持つUI画像のPrefab")]
    public GameObject imagePrefab;

    [Tooltip("貼り付ける画像テクスチャ (Texture2D)")]
    public Texture2D targetImage;

    [Tooltip("画像をインスタンス化する親オブジェクトのTransform")]
    public Transform parent;

    [SerializeField] private string fileName;
    void Update()
    {
        // 1. Meta Questのトリガー（左右どちらか）が押された瞬間をチェック
        bool questTriggerPressed = OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || 
                                   OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger);

        // 2. キーボードの 'O' キーが押された瞬間をチェック（デバッグ用）
        bool keyboardOPressed = Input.GetKeyDown(KeyCode.O);

        // どちらかのボタンが押されたら画像を生成
        if (questTriggerPressed || keyboardOPressed)
        {
            SpawnImage();
        }
    }

    /// <summary>
    /// Prefabをインスタンス化し、RawImageに画像を設定する
    /// </summary>
    public void SpawnImage()
    {
        // 必要なコンポーネントがInspectorから設定されているかチェック
        if (imagePrefab == null)
        {
            Debug.LogError("imagePrefab (画像のPrefab) が設定されていません。");
            return;
        }
        if (targetImage == null)
        {
            Debug.LogError("targetImage (貼り付ける画像) が設定されていません。");
            return;
        }
        if (parent == null)
        {
            Debug.LogError("parent (親オブジェクト) が設定されていません。");
            return;
        }

        // Prefabを親オブジェクト（parent）の子としてインスタンス化
        GameObject newImageObject = Instantiate(imagePrefab, parent);

        // インスタンス化したオブジェクトからRawImageコンポーネントを取得
        RawImage rawImage = newImageObject.GetComponent<RawImage>();

        if (rawImage != null)
        {
            // RawImageのtextureに指定の画像をセット
            rawImage.texture = targetImage;
            
            // （任意）もし画像の元のサイズで表示したい場合は、以下のコメントを解除してください
            // rawImage.SetNativeSize();
        }
        else
        {
            Debug.LogError("指定された 'imagePrefab' に RawImage コンポーネントがありません！");
        }
        
        TextMeshProUGUI tmp = newImageObject.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
        {
            // テキストを設定
            tmp.text = fileName;
        }
        else
        {
            Debug.LogWarning("[ImageLoader] imagePrefab の子に TextMeshProUGUI が見つかりません");
        }
    }
}