using UnityEngine;

public static class ShareMode
{
    public enum ShareMethod
    {
        Hover,
        Ray,
        GUI
    }
    
    private static ShareMethod _setShareMethod = ShareMethod.Hover;
    
    public static ShareMethod GetShareMethod()
    {
        return _setShareMethod;
    }
    
    //基本はUnityEditor上で設定だけど一応
    public static void SetShareMethod(ShareMethod method)
    {
        _setShareMethod = method;
    }
}
