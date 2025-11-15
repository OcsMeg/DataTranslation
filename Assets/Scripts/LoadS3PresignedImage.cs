using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class LoadS3PresignedImage : MonoBehaviour
{
    [Tooltip("S3のpresigned URLをここに設定")]
    public string s3PresignedUrl;

    [Tooltip("表示先のRawImage")]
    public RawImage targetImage;

    void Start()
    {
        StartCoroutine(LoadImageFromS3());
    }

    IEnumerator LoadImageFromS3()
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(s3PresignedUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("S3画像の読み込みに失敗: " + request.error);
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                targetImage.texture = texture;
                Debug.Log("S3から画像を正常に読み込みました。");
            }
        }
    }
}