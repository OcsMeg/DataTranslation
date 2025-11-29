using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI; // RawImageを使う場合

public class SharedImageObject : MonoBehaviour, IPunInstantiateMagicCallback
{
    [SerializeField] private RawImage targetImage; // ここに画像を表示したいRawImageをアサイン
    private ApiClient api;
    private int fileId;

    [System.Serializable]
    public class DownloadInfo
    {
        public string download_url;
        // 他に必要なフィールドがあれば追加
    }

    private void Awake()
    {
        api = GetComponent<ApiClient>();
    }

    // === Photonで生成されたときに一度だけ呼ばれる ===
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        PhotonView pv = GetComponent<PhotonView>();
        object[] data = pv.InstantiationData;

        // 生成時に送った fileId を取得
        fileId = (int)data[0];

        // 各クライアントが自分で画像を取得しに行く
        StartCoroutine(LoadOneFile(fileId));
    }

    IEnumerator LoadOneFile(int id)
    {
        Debug.Log($"=== STEP2: /files/{id} を取得 ===");

        yield return StartCoroutine(api.Get($"/files/{id}", (json) =>
        {
            Debug.Log($"[/files/{id}] Response JSON: " + json);

            DownloadInfo dl = JsonUtility.FromJson<DownloadInfo>(json);
            StartCoroutine(DownloadImage(dl.download_url));
        }));
    }

    IEnumerator DownloadImage(string url)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Image download error: " + uwr.error);
                yield break;
            }

            Texture2D tex = DownloadHandlerTexture.GetContent(uwr);

            if (targetImage != null)
            {
                targetImage.texture = tex;
            }
            else
            {
                // MeshRendererに貼りたい場合の例
                var renderer = GetComponentInChildren<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.material.mainTexture = tex;
                }
            }
        }
    }
}

