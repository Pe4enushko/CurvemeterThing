using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HVRSocket))]
[RequireComponent(typeof(Animator))]
public class ReceiverCoverAnimatorHandler : MonoBehaviour
{
    private HVRSocket receiverCoverSocket;
    private Animator animator;

    private void Start()
    {
        receiverCoverSocket = GetComponent<HVRSocket>();
        animator = GetComponent<Animator>();

        receiverCoverSocket.Grabbed.AddListener(OnGrabbed); 
        receiverCoverSocket.Released.AddListener(OnReleased); 
    }

    private void OnDestroy()
    {
        receiverCoverSocket.Grabbed.RemoveListener(OnGrabbed);
        receiverCoverSocket.Released.RemoveListener(OnReleased);
    }
    private void OnGrabbed(HVRGrabberBase grabberBase, HVRGrabbable grabbable)
    {
        animator.SetBool("IsInstall", true);
    }

    private void OnReleased(HVRGrabberBase grabberBase, HVRGrabbable grabbable)
    {
        animator.SetBool("IsInstall", false);
    }
}
