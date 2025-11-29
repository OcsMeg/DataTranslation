using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
public class CubeFollowTip : MonoBehaviour
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

        var rightHand = subsystem.rightHand;

        if (rightHand.isTracked)
        {
            var indexTip = rightHand.GetJoint(XRHandJointID.IndexTip);

            if (indexTip.TryGetPose(out Pose pose))
            {
                transform.position = pose.position;
            }
        }
    }
}