using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using System.Collections.Generic;
using TMPro;

public class ControllerRaySelect : MonoBehaviour
{
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private LayerMask hitLayers = ~0; // 当てたいレイヤーを設定
    [SerializeField] private LineRenderer line;        // Inspector で割り当て

    // 手のジェスチャ検知
    [SerializeField] private HandGestureDetector handGesture;

    private DataSharing dataSharing;
    private bool isActive = false; // 外部からONにされるまで false

    // XRコントローラー（右手）
    private InputDevice rightController;

    // 前フレームのボタン状態
    private bool prevTriggerPressed = false; // 人差し指トリガー
    private bool prevGripPressed = false;    // 中指グリップ（レイ終了用）
    [SerializeField] private TextMeshProUGUI debugText;

    private void Awake()
    {
        // LineRenderer 自動取得（Inspectorで設定しているなら省略可）
        if (line == null)
        {
            line = GetComponent<LineRenderer>();
        }

        if (line != null)
        {
            line.positionCount = 2;
            line.useWorldSpace = true;
            line.enabled = false;  // 最初は線を非表示
        }

        // ローカルの DataSharing を探す
        foreach (var ds in FindObjectsOfType<DataSharing>())
        {
            var pv = ds.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)
            {
                dataSharing = ds;
                break;
            }
        }

        if (dataSharing == null)
        {
            Debug.LogError("[ControllerRaySelect] ローカルの DataSharing が見つかりません");
        }

        // 右手コントローラー取得
        GetRightController();
    }

    private void Update()
    {
        // 有効化されるまでは何もしない（線も出さない）
        if (!isActive) return;

        // コントローラーが無効になったら再取得
        if (!rightController.isValid)
        {
            GetRightController();
        }

        // ----- 1. Ray を毎フレーム飛ばして線を更新 -----
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        Ray ray = new Ray(origin, direction);
        bool hitSomething = Physics.Raycast(ray, out RaycastHit hit, maxDistance, hitLayers);

        // LineRenderer 用の終点
        Vector3 endPoint = origin + direction * maxDistance;
        if (hitSomething)
        {
            endPoint = hit.point;
        }

        if (line != null)
        {
            line.SetPosition(0, origin);
            line.SetPosition(1, endPoint);
        }

        // ----- 2. ボタン入力取得（コントローラ） -----
        bool triggerPressed = false; // 人差し指トリガー
        bool gripPressed = false;    // 中指グリップ

        rightController.TryGetFeatureValue(CommonUsages.triggerButton, out triggerPressed);
        rightController.TryGetFeatureValue(CommonUsages.gripButton, out gripPressed);

        // === (A) トリガー：false → true になった瞬間だけ決定処理 ===
        if (triggerPressed && !prevTriggerPressed)
        {
            Debug.Log("[ControllerRaySelect] トリガーで決定");
            TrySelectByRay();
        }

        // === (B) グリップ（中指）：false → true になった瞬間にレイ終了 ===
        // if (gripPressed && !prevGripPressed)
        // {
        //     Debug.Log("[ControllerRaySelect] グリップボタンで Ray を終了します");
        //     DeactivateRay();
        // }

        // 今回の状態を次フレーム用に保存
        prevTriggerPressed = triggerPressed;
        prevGripPressed = gripPressed;
    }

    /// <summary>
    /// グーになった瞬間のイベントハンドラ（人差し指トリガーと同じ決定処理を呼ぶ）
    /// </summary>
    public void RightRockDetected()
    {
        if (ShareMode.GetShareMethod() != ShareMode.ShareMethod.Ray) return;
        if (!isActive)
        {
            // Ray がまだ有効じゃないなら何もしない
            return;
        }

        Debug.Log("[ControllerRaySelect] Fist で決定");
        TrySelectByRay();
    }

    /// <summary>
    /// 現在の Ray の方向に対して「決定」処理を行う共通メソッド
    /// </summary>
    private void TrySelectByRay()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        if (!Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, hitLayers))
        {
            Debug.Log("[ControllerRaySelect] 決定しようとしたが、Ray の先にオブジェクトはありません");
            debugText.text = "No Object";
            return;
        }

        PhotonView view = hit.collider.GetComponentInParent<PhotonView>();
        if (view == null)
        {
            Debug.Log("[ControllerRaySelect] PhotonView が見つかりません");
            debugText.text = "No PhotonView";
            return;
        }

        Debug.Log($"[ControllerRaySelect] Hit PhotonView ViewID={view.ViewID}, IsMine={view.IsMine}");

        ReceiveID receiveID = view.GetComponent<ReceiveID>();
        if (receiveID == null)
        {
            Debug.Log("[ControllerRaySelect] ReceiveID コンポーネントが見つかりません");
            debugText.text = "No ReceiveID";
            return;
        }

        string userId = receiveID.GetUserID();
        Debug.Log($"[ControllerRaySelect] Ray で当たったオブジェクトの userID = {userId}");
        debugText.text = userId;

        if (dataSharing != null)
        {
            dataSharing.ReceiveUserID(userId);
            //DeactivateRay();
        }
        else
        {
            Debug.LogError("[ControllerRaySelect] dataSharing が設定されていません");
            debugText.text = "No DataSharing";
        }
    }

    /// <summary>
    /// 外部から呼んで Ray を有効化＆線を表示
    /// </summary>
    public void ActivateRay()
    {
        isActive = true;
        if (line != null)
        {
            line.enabled = true;
        }
    }

    /// <summary>
    /// Ray を停止＆線を非表示
    /// </summary>
    public void DeactivateRay()
    {
        isActive = false;
        if (line != null)
        {
            line.enabled = false;
        }
        dataSharing.StopSharing();
    }

    // 右手コントローラー取得
    private void GetRightController()
    {
        var devices = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller,
            devices);

        if (devices.Count > 0)
        {
            rightController = devices[0];
            Debug.Log($"[ControllerRaySelect] 右コントローラー取得: {rightController.name}");
        }
        else
        {
            Debug.Log("[ControllerRaySelect] 右コントローラーが見つかりませんでした");
        }
    }
}
