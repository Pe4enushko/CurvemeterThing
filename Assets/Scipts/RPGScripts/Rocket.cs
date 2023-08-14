using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [InspectorName("Type")]
    public RocketTypes RocketType = RocketTypes.None;
    [HideInInspector]
    public bool isAttached = false;
}
