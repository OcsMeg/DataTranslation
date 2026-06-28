using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

public class LeftFollowPalm : MonoBehaviour
{
    XRHandSubsystem subsystem;

    [SerializeField] private HandUIRaycaster handUIRaycaster; // ★ 右手側の HandUIRaycaster をアサイン

    void Start()
    {
        subsystem = XRGeneralSettings.Instance?
            .Manager?
            .activeLoader?
            .GetLoadedSubsystem<XRHandSubsystem>();
    }

    void Update()
    {
        // UI を押している間は位置・回転を更新しない
        if (handUIRaycaster != null && handUIRaycaster.IsPressingUI)
            return;

        if (subsystem == null) return;

        var left = subsystem.leftHand;
        if (!left.isTracked) return;

        var palm = left.GetJoint(XRHandJointID.Palm);

        if (palm.TryGetPose(out Pose pose))
        {
            transform.SetPositionAndRotation(pose.position, pose.rotation);
        }
    }
}