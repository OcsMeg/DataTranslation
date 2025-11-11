using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeSelectManager : MonoBehaviour
{
    private bool isModeSelected = false; //選ばれたかどうかのフラグ
    [SerializeField] private SelectModeUI selectedModeUI; //モード選択UI

    private int selectIndex = 0; //選択されているキャラのインデックス

    private PlayerMode.PlayMode[] modelists = new PlayerMode.PlayMode[]
    {
        PlayerMode.PlayMode.VR,
        PlayerMode.PlayMode.MR
    };
    
    void Start()
    {
        //初期状態のUIを更新
        UpdateSelectionUI();
    }
    
    void Update()
    {
        //左の中指ボタンで左に移動
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveSelectionLeft();
        } 
        //右の中指ボタンで右に移動
        else if (OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveSelectionRight();
        }
        
        if ((OVRInput.Get(OVRInput.Button.One) || Input.GetKeyDown(KeyCode.Return)))
        {
            SelectMode(modelists[selectIndex]);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            SelectMode(PlayerMode.PlayMode.GOD);
        }
    }
    
    private void MoveSelectionLeft()
    {
        selectIndex--;
        if (selectIndex < 0)
        {
            selectIndex = modelists.Length - 1; // 一番右にループ
        }
        UpdateSelectionUI();
        Debug.Log($"Current Selection: {modelists[selectIndex]}");
    }

    private void MoveSelectionRight()
    {
        selectIndex++;
        if (selectIndex >= modelists.Length)
        {
            selectIndex = 0; // 一番左にループ
        }
        UpdateSelectionUI();
        Debug.Log($"Current Selection: {modelists[selectIndex]}");
    }

    private void UpdateSelectionUI()
    {
        // 現在の選択に応じてUIを更新
        switch (modelists[selectIndex])
        {
            case PlayerMode.PlayMode.VR:
                selectedModeUI.VRSelected();
                break;
            case PlayerMode.PlayMode.MR:
                selectedModeUI.MRSelected();
                break;
        }
    }
    
    private void SelectMode(PlayerMode.PlayMode mode)
    {
        PlayerMode.SetSelectedPlayMode(mode);
        isModeSelected = true;
        Debug.Log($"Selected Mode: {mode}");

        // チュートリアルシーンに移行
        GoToPlayScene();
    }

    private void GoToPlayScene()
    {
        // チュートリアルシーンに遷移する処理
        SceneManager.LoadScene("PlayScene");
        Debug.Log("Go to Play Scene");
    }
}
