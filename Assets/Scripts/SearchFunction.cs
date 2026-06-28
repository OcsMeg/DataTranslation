using UnityEngine;

public class SearchFunction : MonoBehaviour
{
    private DisplayToggleController _displayToggleController;
    private ControllerRaySelect _controllerRaySelect;
    private ShareCancel _shareCancel;
    private CardManager _cardManager;

    // player を渡してもらって参照を作る
    public void SetFunction(GameObject player)
    {
        if (player == null)
        {
            Debug.LogError("[SearchFunction] player is null.");
            return;
        }

        // player に付いている DisplayToggleController を取る
        _displayToggleController = player.GetComponent<DisplayToggleController>();
        if (_displayToggleController == null)
        {
            // 子に付いている可能性があるならこちらに差し替え
            // _displayToggleController = player.GetComponentInChildren<DisplayToggleController>(true);

            Debug.LogError("[SearchFunction] DisplayToggleController not found on player.");
        }

        // SearchFunction が付いてる（＝camera側）から ControllerRaySelect を取る
        _controllerRaySelect = player.GetComponentInChildren<ControllerRaySelect>();
        if (_controllerRaySelect == null)
        {
            Debug.LogWarning("[SearchFunction] ControllerRaySelect not found on camera(SearchFunction owner).");
        }
        
        _shareCancel = player.GetComponentInChildren<ShareCancel>();
        if (_shareCancel == null)
        {
            Debug.LogWarning("[SearchFunction] ShareCancel not found on camera(SearchFunction owner).");
        }
        
        _cardManager = player.GetComponentInChildren<CardManager>();
        if (_cardManager == null)
        {
            Debug.LogWarning("[SearchFunction] CardManager not found on camera(SearchFunction owner).");
        }
    }

    public void RockFunction()
    {
        if (_displayToggleController == null)
        {
            Debug.LogWarning("[SearchFunction] _displayToggleController is null. Did you call SetFunction(player)?");
            return;
        }
        _displayToggleController.ChangeToggleCanvas();
    }

    public void ScissorsFunction()
    {
        if (_shareCancel == null)
        {
            Debug.LogWarning("[SearchFunction] _shareCancel is null. Did you call SetFunction(player)?");
            return;
        }
        _shareCancel.HandleScissors();
    }

    public void RayFunction()
    {
        if (_controllerRaySelect == null)
        {
            Debug.LogWarning("[SearchFunction] _controllerRaySelect is null. Did you call SetFunction(player)?");
            return;
        }
        _controllerRaySelect.RightRockDetected();
    }

    public void HoverReceiveFunction()
    {
        if (_cardManager == null)
        {
            Debug.LogWarning("[SearchFunction] _cardManager is null. Did you call SetFunction(player)?");
            return;
        }
        _cardManager.HandleRightHandFist();
    }
}