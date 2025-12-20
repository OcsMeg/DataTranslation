using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShareButton : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button shareButton;
    [SerializeField] private Transform imageParent; // 画像（Toggle付き）の親オブジェクト

    private List<Toggle> toggles = new List<Toggle>();
    
    // デバッグ用
    //[SerializeField] private TextMeshProUGUI shareText;
    private DataSharing dataSharing;

    void Start()
    {
        GameObject playerRoot = GameObject.FindGameObjectWithTag("PlayerRoot");
        dataSharing = playerRoot.GetComponent<DataSharing>();
    }
    
    private void SearchToggle()
    {
        toggles.Clear();
        // 親の子から Toggle を全て取得
        foreach (Transform child in imageParent)
        {
            Toggle tg = child.GetComponentInChildren<Toggle>();
            if (tg != null)
            {
                toggles.Add(tg);
                tg.onValueChanged.AddListener(OnToggleChanged);
            }
        }

        // 初期状態ではボタン無効
        shareButton.interactable = false;
        shareButton.onClick.AddListener(OnShareButtonClicked);
    }

    public void SetSearchToggle()
    {
        SearchToggle();
    }
    /// <summary>
    /// トグル状態が変化したときに呼ばれる
    /// 少なくとも1つONならボタン有効化
    /// </summary>
    private void OnToggleChanged(bool _)
    {
        bool anySelected = toggles.Exists(t => t.isOn);
        shareButton.interactable = anySelected;
    }

    /// <summary>
    /// ボタンが押されたとき、選択された画像の TextMeshPro を読み取る
    /// </summary>
    private void OnShareButtonClicked()
    {
        List<int> selectedID = new List<int>();

        foreach (Toggle tg in toggles)
        {
            if (tg.isOn)
            {
                // 子オブジェクトから FileIDMemory を探す
                FileIDMemory id = tg.GetComponentInChildren<FileIDMemory>();

                if (id != null)
                {
                    selectedID.Add(id.GetFileID());
                }
            }
        }
        DataSharing(selectedID);
    }

    /// <summary>
    /// データ共有処理（あなたが実装）
    /// </summary>
    private void DataSharing(List<int> selectedID)
    {
        // 選択状態をクリア（イベントを発火させない）
        foreach (var tg in toggles)
        {
            if (tg != null) tg.SetIsOnWithoutNotify(false);
        }
        
        shareButton.interactable = false;
        dataSharing.StartSharing(selectedID);
    }
}