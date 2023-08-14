using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HurricaneVR;
using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Core;
using System;

public class GunPartsAssembly : MonoBehaviour
{
    //RecieverCover
    [Header("RecieverCover")]
    [SerializeField]
    private HVRSocket receiverCoverSocket;
    [SerializeField]
    private GameObject receiverCoverGrab;
    [SerializeField]
    private GameObject receiverCoverTrigger;

    //Spring
    [Header("Spring")]
    [SerializeField]
    private HVRSocket springSocket;
    [SerializeField]
    private GameObject springGrab;
    [SerializeField]
    private GameObject springTrigger;

    //BoltCarrier
    [Header("BoltCarrier")]
    [SerializeField]
    private HVRSocket boltCarrierSocket;
    [SerializeField]
    private GameObject boltCarrierGrab;
    [SerializeField]
    private GameObject boltCarrierTrigger;


    [SerializeField]
    private HVRGrabbable gunCharger;
    [SerializeField]
    private GunBoltCarrier currentBoltCarrier;

    private void Start()
    {
        receiverCoverSocket.Grabbed.AddListener(OnGrabbedReceiver);
        receiverCoverSocket.Released.AddListener(OnReleasedReceiver);
        
        springSocket.Grabbed.AddListener(OnGrabbedSpring);
        springSocket.Released.AddListener(OnReleasedSpring);
        
        boltCarrierSocket.Grabbed.AddListener(OnGrabbedBoltCarrier);
        boltCarrierSocket.Released.AddListener(OnReleasedBoltCarrier);

        gunCharger.CanBeGrabbed = boltCarrierSocket.transform.childCount > 0;
    }

    private void OnDestroy()
    {
        receiverCoverSocket.Grabbed.RemoveListener(OnGrabbedReceiver);
        receiverCoverSocket.Released.RemoveListener(OnReleasedReceiver);

        springSocket.Grabbed.RemoveListener(OnGrabbedSpring);
        springSocket.Released.RemoveListener(OnReleasedSpring);

        boltCarrierSocket.Grabbed.RemoveListener(OnGrabbedBoltCarrier);
        boltCarrierSocket.Released.RemoveListener(OnReleasedBoltCarrier);
    }

    private void OnGrabbedBoltCarrier(HVRGrabberBase grabberBase, HVRGrabbable grabbable)
    {
        Debug.Log("OnGrabbedBoltCarrier");
        currentBoltCarrier  = grabbable.GetComponent<GunBoltCarrier>();
        if(currentBoltCarrier == null)
        {
            Debug.LogWarning("BoltCarrier component not found");
        }
        else
        {
            currentBoltCarrier.Lock();
        }

        springTrigger.SetActive(receiverCoverSocket.transform.childCount == 0);
        springGrab.SetActive(receiverCoverSocket.transform.childCount == 0);

        gunCharger.CanBeGrabbed = true;
    }

    private void OnReleasedBoltCarrier(HVRGrabberBase grabberBase, HVRGrabbable grabbable)
    {
        Debug.Log("OnReleasedBoltCarrier");
        currentBoltCarrier.Unlock();
        currentBoltCarrier = null;

        springTrigger.SetActive(false);
        springGrab.SetActive(false);

        gunCharger.CanBeGrabbed = false;
    }

    private void OnGrabbedSpring(HVRGrabberBase grabberBase, HVRGrabbable grabbable)
    {
        Debug.Log("OnGrabbedSpring");
        boltCarrierGrab.SetActive(false);
        boltCarrierTrigger.SetActive(false);
    }

    private void OnReleasedSpring(HVRGrabberBase grabberBase, HVRGrabbable grabbable)
    {
        Debug.Log("OnReleasedSpring");
        boltCarrierGrab.SetActive(true);
        boltCarrierTrigger.SetActive(true);
    }

    private void OnGrabbedReceiver(HVRGrabberBase grabberBase, HVRGrabbable grabbable)
    {
        Debug.Log("OnGrabbedReceiver");
        springTrigger.SetActive(false);
        springGrab.SetActive(false);

        boltCarrierGrab.SetActive(false);
        boltCarrierTrigger.SetActive(false);
    }

    private void OnReleasedReceiver(HVRGrabberBase grabberBase, HVRGrabbable grabbable)
    {
        Debug.Log("OnReleasedReceiver");
        boltCarrierGrab.SetActive(springSocket.transform.childCount.Equals(0));
        boltCarrierTrigger.SetActive(springSocket.transform.childCount.Equals(0));

        springTrigger.SetActive(!boltCarrierSocket.transform.childCount.Equals(0));
        springGrab.SetActive(true);
    }
}
