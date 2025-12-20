using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectShareModeUI : MonoBehaviour
{
    [SerializeField] private RawImage HoverFrameUI;
    [SerializeField] private TextMeshProUGUI HoverFrameText;
    [SerializeField] private RawImage RayFrameUI;
    [SerializeField] private TextMeshProUGUI RayFrameText;
    [SerializeField] private RawImage GUIFrameUI;
    [SerializeField] private TextMeshProUGUI GUIFrameText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void ResetUI()
    {
        HoverFrameUI.color = Color.white;
        HoverFrameText.color = Color.white;
        RayFrameUI.color = Color.white;
        RayFrameText.color = Color.white;
        GUIFrameUI.color = Color.white;
        GUIFrameText.color = Color.white;
    }

    public void HoverSelected()
    {
        HoverFrameUI.color = Color.yellow;
        HoverFrameText.color = Color.yellow;
        RayFrameUI.color = Color.white;
        RayFrameText.color = Color.white;
        GUIFrameUI.color = Color.white;
        GUIFrameText.color = Color.white;
        Debug.Log("Hover Selected");
    }

    public void RaySelected()
    {
        HoverFrameUI.color = Color.white;
        HoverFrameText.color = Color.white;
        RayFrameUI.color = Color.yellow;
        RayFrameText.color = Color.yellow;
        GUIFrameUI.color = Color.white;
        GUIFrameText.color = Color.white;
        Debug.Log("Ray Selected");
    }
    
    public void GUISelected()
    {
        HoverFrameUI.color = Color.white;
        HoverFrameText.color = Color.white;
        RayFrameUI.color = Color.white;
        RayFrameText.color = Color.white;
        GUIFrameUI.color = Color.yellow;
        GUIFrameText.color = Color.yellow;
        Debug.Log("GUI Selected");
    }
}
