using System;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

public class HandGestureDetector : MonoBehaviour
{
    public bool useRightHand = true;      // true = 右手, false = 左手

    [Header("しきい値")]
    [Tooltip("指先と手のひらの距離がこの値より小さいとグーと判定")]
    public float fistDistanceThreshold = 0.05f;

    /// <summary> グーになった瞬間に呼ばれるイベント </summary>
    public event Action OnFist;

    /// <summary> 現在の手がトラッキングされているか </summary>
    public bool IsHandTracked { get; private set; }

    /// <summary> 最新の手のひらのポーズ（位置・回転） </summary>
    public Pose PalmPose { get; private set; }

    XRHandSubsystem _subsystem;
    bool _prevIsFist = false;
    
    public Pose IndexTipPose { get; private set; }
    

    void Start()
    {
        TryInitSubsystem();
    }

    void TryInitSubsystem()
    {
        _subsystem = XRGeneralSettings.Instance?
            .Manager?
            .activeLoader?
            .GetLoadedSubsystem<XRHandSubsystem>();

        if (_subsystem == null)
        {
            Debug.LogWarning("XRHandSubsystem が見つかりません。Hand Tracking が有効か確認してください。");
        }
    }

    void Update()
    {
        if (_subsystem == null)
        {
            TryInitSubsystem();
            if (_subsystem == null) return;
        }

        XRHand hand = useRightHand ? _subsystem.rightHand : _subsystem.leftHand;

        if (!hand.isTracked)
        {
            IsHandTracked = false;
            _prevIsFist = false;
            return;
        }

        IsHandTracked = true;

        // 手のひらのポーズ取得
        if (!TryGetJointPose(hand, XRHandJointID.Palm, out Pose palmPose))
        {
            IsHandTracked = false;
            _prevIsFist = false;
            return;
        }

        PalmPose = palmPose;

        // 各指先のポーズ
        TryGetJointPose(hand, XRHandJointID.IndexTip,  out Pose indexTip);
        TryGetJointPose(hand, XRHandJointID.MiddleTip, out Pose middleTip);
        TryGetJointPose(hand, XRHandJointID.RingTip,   out Pose ringTip);
        TryGetJointPose(hand, XRHandJointID.LittleTip, out Pose littleTip);

        // 手のひらとの距離
        float indexDist  = Vector3.Distance(indexTip.position,  palmPose.position);
        float middleDist = Vector3.Distance(middleTip.position, palmPose.position);
        float ringDist   = Vector3.Distance(ringTip.position,   palmPose.position);
        float littleDist = Vector3.Distance(littleTip.position, palmPose.position);
        
        IndexTipPose = indexTip;
        
        // 全ての指先が手のひらに近ければグーと判定
        bool isFist =
            indexDist  < fistDistanceThreshold &&
            middleDist < fistDistanceThreshold &&
            ringDist   < fistDistanceThreshold &&
            littleDist < fistDistanceThreshold;

        // グー「になった瞬間」だけイベントを飛ばす
        if (isFist && !_prevIsFist)
        {
            Debug.Log("Fist detected!");
            OnFist?.Invoke();
        }

        _prevIsFist = isFist;
    }

    bool TryGetJointPose(XRHand hand, XRHandJointID jointId, out Pose pose)
    {
        XRHandJoint joint = hand.GetJoint(jointId);
        if (joint.TryGetPose(out pose))
        {
            return true;
        }
        return false;
    }
}
