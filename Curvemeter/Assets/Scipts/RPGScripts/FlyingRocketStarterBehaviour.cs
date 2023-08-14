using HurricaneVR.Framework.Core.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingRocketStarterBehaviour : RocketStarterBehaviour
{
    void Awake()
    {
        if (transform.parent != null)
        {
            AttachedRocket = transform.parent
                .gameObject
                .GetComponentInChildren<Rocket>()
                .GetRigidbody();
        }
    }
}
