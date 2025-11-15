using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ApiClient : MonoBehaviour
{
    private string baseUrl = "http://10.40.22.36:8000"; // FastAPIのURL

    public IEnumerator Post(string endpoint, string jsonData, System.Action<string> callback)
    {
        string url = baseUrl + endpoint;
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            callback?.Invoke(request.downloadHandler.text);
        else
            Debug.LogError($"POST Error: {request.error}\n{request.downloadHandler.text}");
    }

    public IEnumerator Get(string endpoint, System.Action<string> callback)
    {
        string url = baseUrl + endpoint;
        var request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            callback?.Invoke(request.downloadHandler.text);
        else
            Debug.LogError($"GET Error: {request.error}\n{request.downloadHandler.text}");
    }
}

