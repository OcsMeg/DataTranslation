using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

public class RightFollowPalm : MonoBehaviour
{
    XRHandSubsystem subsystem;

    void Start()
    {
        subsystem = XRGeneralSettings.Instance?
            .Manager?
            .activeLoader?
            .GetLoadedSubsystem<XRHandSubsystem>();
    }

    void Update()
    {
        if (subsystem == null) return;

        var right = subsystem.rightHand;
        if (!right.isTracked) return;

        var palm = right.GetJoint(XRHandJointID.Palm);

        if (palm.TryGetPose(out Pose pose))
        {
            // 手のひらの位置と向きに追従
            transform.SetPositionAndRotation(pose.position, pose.rotation);
        }
    }
}
