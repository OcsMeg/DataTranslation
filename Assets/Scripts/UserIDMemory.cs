using UnityEngine;

public class UserIDMemory : MonoBehaviour
{
    private string userID;
    
    public void SetUserID(string userName)
    {
        userID = userName;
    }

    public string GetUserID()
    {
        return userID;
    }
}
