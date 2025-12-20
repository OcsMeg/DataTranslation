using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIMethod : MonoBehaviour
{
    [SerializeField] private Transform parent;
    [SerializeField] private Canvas displayCanvas;
    [SerializeField] private GameObject imagePrefab;
    [SerializeField] private Texture2D iconTexture;
    [SerializeField] private Button shareButton;
    [SerializeField] private Button shareGUIButton;
    [SerializeField] private GUIShareButton shareScript;
    [SerializeField] private TextMeshProUGUI debugText;

    
    private void ClearImages()
    {
        if (parent == null) return;

        foreach (Transform child in parent)
        {
            child.gameObject.SetActive(false); // 完全に消したいなら Destroy(child.gameObject);
        }
    }
    
    public void SelectRecipient()
    {
        
        ClearImages();
        var userList = GameManager.GetRoomUserNameList();
        shareButton.interactable = false;
        //shareButton.enabled = false;
        //shareGUIButton.enabled = true;
        foreach (var user in userList)
        {
            if (user == PlayerMode.GetPlayerName()) continue;
            
            // Prefab 作成
            GameObject obj = Instantiate(imagePrefab, parent);
            RawImage img = obj.GetComponent<RawImage>();
            img.texture = iconTexture;
    
            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(300, 300);
    
            TextMeshProUGUI tmp = obj.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) tmp.text = user;
            
            UserIDMemory mem = obj.GetComponent<UserIDMemory>();
            mem.SetUserID(user);
        }
        shareScript.SetSearchToggle();
    }

    public void CancelShare()
    {
        //シェア用キャンバスを閉じる
        ClearImages();
        displayCanvas.enabled = false;
    }
}
