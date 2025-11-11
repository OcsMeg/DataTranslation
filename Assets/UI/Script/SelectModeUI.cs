using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SelectModeUI : MonoBehaviour
{
    [SerializeField] private RawImage VRFrameUI;
    [SerializeField] private TextMeshProUGUI VRFrameText;
    [SerializeField] private RawImage MRFrameUI;
    [SerializeField] private TextMeshProUGUI MRFrameText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void ResetUI()
    {
        VRFrameUI.color = Color.white;
        VRFrameText.color = Color.white;
        MRFrameUI.color = Color.white;
        MRFrameText.color = Color.white;
    }

    public void VRSelected()
    {
        VRFrameUI.color = Color.yellow;
        VRFrameText.color = Color.yellow;
        MRFrameUI.color = Color.white;
        MRFrameText.color = Color.white;
        Debug.Log("VR Selected");
    }

    public void MRSelected()
    {
        VRFrameUI.color = Color.white;
        VRFrameText.color = Color.white;
        MRFrameUI.color = Color.yellow;
        MRFrameText.color = Color.yellow;
        Debug.Log("MR Selected");
    }
}
