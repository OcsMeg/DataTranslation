using UnityEngine;

public static class PlayerMode
{
    public enum PlayMode
    {
        VR,
        MR,
        GOD
    }
    private static PlayMode _setplayMode = PlayMode.VR; //デフォルトはVR

    public static PlayMode GetSelectedPlayMode()
    {
        return _setplayMode;
    }
    
    public static void SetSelectedPlayMode(PlayMode playMode)
    {
        _setplayMode = playMode;
    }
    
    private static string myName = "Player";
    public static string GetPlayerName()
    {
        return myName;
    }
    
    public static void SetPlayerName(string name)
    {
        myName = name;
    }

    public static void SetLayer(GameObject gameobject, int layer)
    {
        gameobject.layer = layer;
        foreach (Transform child in gameobject.transform)
        {
            SetLayer(child.gameObject, layer);
        }
    }
}
