using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CreatePhotonAvatar : MonoBehaviourPunCallbacks
{
    private GameObject masterPlayerObject;
    private GameObject[]    rootTargets = new GameObject[3];
    private GameObject _stateManager;
    private CharacterController _controller;
    [SerializeField] GameObject[] Targets = new GameObject[3];
    private bool isCreated = false;

    private void OnCreate()
    {

        rootTargets[0] = GameObject.FindGameObjectWithTag("CameraRig");
        if (rootTargets[0] == null)
        {
            Debug.LogError("CameraRig object not found! Check the tag and its active status in the scene.");
        }
        else
        {
            rootTargets[0].SetActive(true);
        }

        // rootTargets[1] = GameObject.FindGameObjectWithTag("TrackingSpace");
        // if (rootTargets[1] == null)
        // {
        //     Debug.LogError("TrackingSpace object not found! Check the tag and its active status in the scene.");
        // }
        // else
        // {
        //     rootTargets[1].SetActive(true);
        // }

        // rootTargets[1] を LHandTargetAnchor で取得
        rootTargets[1] = GameObject.FindGameObjectWithTag("LHandTargetAnchor");
        if (rootTargets[1] == null)
        {
            Debug.LogError("LHandTargetAnchor object not found! Check the tag and its active status in the scene.");
        }
        else
        {
            rootTargets[1].SetActive(true);
        }

        // rootTargets[2] を RHandTargetAnchor で取得
        rootTargets[2] = GameObject.FindGameObjectWithTag("RHandTargetAnchor");
        if (rootTargets[2] == null)
        {
            Debug.LogError("RHandTargetAnchor object not found! Check the tag and its active status in the scene.");
        }
        else
        {
            rootTargets[2].SetActive(true);
        }
        
        if (rootTargets[0] != null && rootTargets[1] != null && rootTargets[2] != null)
        {
            isCreated = true;
        }
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (isCreated)
        {
            for (int i = 0; i < 3; i++)
            {
                Targets[i].transform.position = rootTargets[i].transform.position;
                Targets[i].transform.rotation = rootTargets[i].transform.rotation;
            }

        }
    }

    public void ExecuteCreatePhotonAvatar()
    {
        OnCreate();
        Debug.Log("CreatePhotonAvatar");
    }
}