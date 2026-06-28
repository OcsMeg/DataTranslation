using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GUIShareButton : MonoBehaviour
{
    [SerializeField] private Button shareButton;
    [SerializeField] private Transform imageParent; // 画像（Toggle付き）の親オブジェクト
    [SerializeField] private TextMeshProUGUI debugText;

    private List<Toggle> toggles = new List<Toggle>();
    private DataSharing dataSharing;
    [SerializeField] private ImageLoader imageLoader;
    void Start()
    {
        GameObject playerRoot = GameObject.FindGameObjectWithTag("PlayerRoot");
        dataSharing = playerRoot.GetComponent<DataSharing>();
        shareButton.interactable = false;
    }

    private void SearchToggle()
    {
        toggles.Clear();
        // 親の子から Toggle を全て取得
        foreach (Transform child in imageParent)
        {
            Toggle tg = child.GetComponent<Toggle>();
            if (tg != null)
            {
                toggles.Add(tg);
                
                // 二重登録防止
                tg.onValueChanged.RemoveListener(OnToggleChanged);
                tg.onValueChanged.AddListener(OnToggleChanged);
            }
        }
        // 初期状態ではボタン無効
        shareButton.interactable = false;
        // 2重登録防止
        shareButton.onClick.RemoveListener(OnShareButtonClicked);
        shareButton.onClick.AddListener(OnShareButtonClicked);
    }
    
    public void SetSearchToggle()
    {
        SearchToggle();
    }
    
    private void OnToggleChanged(bool _)
    {
        bool anySelected = toggles.Exists(t => t.isOn);
        shareButton.interactable = anySelected;
    }
    
    private void OnShareButtonClicked()
    {
        foreach (Toggle tg in toggles)
        {
            if (tg.isOn)
            {
                // 子オブジェクトから FileIDMemory を探す
                UserIDMemory id = tg.GetComponentInChildren<UserIDMemory>();

                if (id != null)
                {
                    //selectedID.Add(id.GetUserID());
                    DataSharing(id.GetUserID());
                }
            }
        }
        imageLoader.CloseDisplayCanvas();
        shareButton.interactable = false;
        dataSharing.StopSharing();
    }
    private void DataSharing(string userID)
    {
        dataSharing.ReceiveUserID(userID);
    }
}
