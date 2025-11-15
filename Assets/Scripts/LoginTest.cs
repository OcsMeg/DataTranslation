using UnityEngine;

public class LoginTest : MonoBehaviour
{
    private ApiClient api;

    void Start()
    {
        api = gameObject.AddComponent<ApiClient>();
        StartCoroutine(api.Post("/login?username=admin&password=test", "{}", 
            (response) => Debug.Log("Login Response: " + response)));
    }
}

