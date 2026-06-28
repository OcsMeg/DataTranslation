using UnityEngine;

public class SelectShareMode : MonoBehaviour
{
    public enum ShareModeType
    {
        Hover,
        Ray,
        GUI
    }
    [SerializeField] private ShareModeType _shareMode = ShareModeType.Hover;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GetShareMode() == ShareModeType.Hover)
        {
            ShareMode.SetShareMethod(ShareMode.ShareMethod.Hover);
        } else if (GetShareMode() == ShareModeType.Ray)
        {
            ShareMode.SetShareMethod(ShareMode.ShareMethod.Ray);
        } else if (GetShareMode() == ShareModeType.GUI)
        {
            ShareMode.SetShareMethod(ShareMode.ShareMethod.GUI);
        }
    }
    
    private ShareModeType GetShareMode()
    {
        return _shareMode;
    }
}
