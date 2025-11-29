using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

public class XRHandTest : MonoBehaviour
{
    XRHandSubsystem subsystem;

    void Start()
    {
        subsystem = XRGeneralSettings.Instance?
            .Manager?
            .activeLoader?
            .GetLoadedSubsystem<XRHandSubsystem>();

        if (subsystem == null)
            Debug.LogError("XRHandSubsystem が見つかりません");
    }

    void Update()
    {
        if (subsystem == null) return;

        var right = subsystem.rightHand;

        if (right.isTracked)
        {
            var joint = right.GetJoint(XRHandJointID.IndexTip);
            if (joint.TryGetPose(out Pose pose))
            {
                Debug.Log("Right Index Tip → " + pose.position);
            }
        }
    }
}