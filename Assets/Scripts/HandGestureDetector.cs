using System;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

public class HandGestureDetector : MonoBehaviour
{
    public bool useRightHand = true;      // true = 右手, false = 左手

    [Header("しきい値（距離）")]
    [Tooltip("指先と手のひらの距離がこの値より小さいと「曲がっている（握っている）」と判定")]
    public float fistDistanceThreshold = 0.03f;

    [Tooltip("指先と手のひらの距離がこの値より大きいと「伸びている」と判定（チョキ用）")]
    public float extendedDistanceThreshold = 0.08f;

    /// <summary> グーになった瞬間に呼ばれるイベント </summary>
    public event Action OnFist;

    /// <summary> チョキになった瞬間に呼ばれるイベント </summary>
    public event Action OnScissors;

    /// <summary> 現在の手がトラッキングされているか </summary>
    public bool IsHandTracked { get; private set; }

    /// <summary> 最新の手のひらのポーズ（位置・回転） </summary>
    public Pose PalmPose { get; private set; }

    public Pose IndexTipPose { get; private set; }
    public Pose MiddleTipPose { get; private set; }

    XRHandSubsystem _subsystem;

    bool _prevIsFist = false;
    bool _prevIsScissors = false;
    
    [SerializeField] private TextMeshProUGUI debugText;
    //デバックよう
    float _logTimer = 0f;

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
            _prevIsScissors = false;
            return;
        }

        IsHandTracked = true;

        // 手のひらのポーズ取得
        if (!TryGetJointPose(hand, XRHandJointID.Palm, out Pose palmPose))
        {
            IsHandTracked = false;
            _prevIsFist = false;
            _prevIsScissors = false;
            return;
        }
        PalmPose = palmPose;

        // 各指先のポーズ（取れなければ判定しない）
        if (!TryGetJointPose(hand, XRHandJointID.IndexTip, out Pose indexTip) ||
            !TryGetJointPose(hand, XRHandJointID.MiddleTip, out Pose middleTip) ||
            !TryGetJointPose(hand, XRHandJointID.RingTip, out Pose ringTip) ||
            !TryGetJointPose(hand, XRHandJointID.LittleTip, out Pose littleTip))
        {
            _prevIsFist = false;
            _prevIsScissors = false;
            return;
        }

        IndexTipPose = indexTip;
        MiddleTipPose = middleTip;

        // 手のひらとの距離
        float indexDist  = Vector3.Distance(indexTip.position,  palmPose.position);
        float middleDist = Vector3.Distance(middleTip.position, palmPose.position);
        float ringDist   = Vector3.Distance(ringTip.position,   palmPose.position);
        float littleDist = Vector3.Distance(littleTip.position, palmPose.position);
        
        // ===== チョキ判定（人差し指・中指が伸びて、薬指・小指が曲がっている）=====
        //         bool isScissors =
        //             indexDist  > extendedDistanceThreshold &&
        //             middleDist > extendedDistanceThreshold &&
        //             ringDist   < fistDistanceThreshold &&
        //             littleDist < fistDistanceThreshold;
        //
        // ===== グー判定（全指が曲がっている）=====
        // bool isFist =
        //     indexDist  < fistDistanceThreshold &&
        //     middleDist < fistDistanceThreshold &&
        //     ringDist   < fistDistanceThreshold &&
        //     littleDist < fistDistanceThreshold;
        //
        // イベント：チョキ「になった瞬間」
        // if (isScissors && !_prevIsScissors)
        // {
        //     Debug.Log("Scissors detected!");
        //     OnScissors?.Invoke();
        // }
        //
        // イベント：グー「になった瞬間」
        // if (isFist && !_prevIsFist)
        // {
        //     Debug.Log("Fist detected!");
        //     OnFist?.Invoke();
        // }

        

        // _prevIsFist = isFist;
        // _prevIsScissors = isScissors;
        
        // _logTimer += Time.deltaTime;
        // if (_logTimer > 0.2f)
        // {
        //     _logTimer = 0f;
        //     debugText.text = $"index:{indexDist:F3} middle:{middleDist:F3} ring:{ringDist:F3} little:{littleDist:F3}";
        //     Debug.Log($"index:{indexDist:F3} middle:{middleDist:F3} ring:{ringDist:F3} little:{littleDist:F3}");
        // }
    }

    bool TryGetJointPose(XRHand hand, XRHandJointID jointId, out Pose pose)
    {
        XRHandJoint joint = hand.GetJoint(jointId);
        return joint.TryGetPose(out pose);
    }
}
