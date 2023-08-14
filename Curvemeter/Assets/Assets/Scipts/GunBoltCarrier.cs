using HurricaneVR.Framework.Core.Grabbers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBoltCarrier : MonoBehaviour
{
    [SerializeField]
    private HVRSocket boltCarrierSocket;
    [SerializeField]
    private GameObject receiverCoverGrab;
    [SerializeField]
    private GameObject receiverCoverTrigger;

    public void Lock()
    {
        receiverCoverGrab.SetActive(false);
        receiverCoverTrigger.SetActive(false);
    }

    public void Unlock()
    {
        receiverCoverGrab.SetActive(true);
        receiverCoverTrigger.SetActive(true);
    }
}
