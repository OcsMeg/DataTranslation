using UnityEngine;
using Photon.Pun;
public class AvatarController : MonoBehaviour
{
    public Transform leftHandTarget;
    public Transform rightHandTarget;
    public Transform pointView;
    public Transform cameraRig;
    private Animator animator;
    private Quaternion edit_Rightcamera;
    private Quaternion edit_Leftcamera;
    private const float heightDiffPandaCamera = 1.55f; // アバタとカメラの高さの差分
    
    void Start()
    {
        animator = GetComponent<Animator>();
        edit_Rightcamera = Quaternion.Euler(0f, 0f, -90f);
        edit_Leftcamera = Quaternion.Euler(0f, 0f, 90f);
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            // アバタの位置をカメラに合わせる
            Vector3 cameraPosition = cameraRig.position;
            cameraPosition.y -= heightDiffPandaCamera; // アバタの高さを調整
            transform.position = cameraPosition;
        
            // パンダの向きをカメラの向きに合わせる
            Quaternion targetRotation = Quaternion.Euler(0, cameraRig.transform.eulerAngles.y, 0);
            transform.rotation = targetRotation;
        }
    }
    
    void OnAnimatorIK()
    {
        if (pointView != null)
        {
            animator.SetLookAtWeight(1);
            animator.SetLookAtPosition(pointView.position);
        }
        else
        {
            animator.SetLookAtWeight(0);
        }

        // 右手
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation * edit_Rightcamera);

        // 左手
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation * edit_Leftcamera);
    }

}
