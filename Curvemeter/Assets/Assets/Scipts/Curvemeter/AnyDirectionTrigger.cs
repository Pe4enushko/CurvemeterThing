using Assets.Assets.Scipts.Curvemeter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnyDirectionTrigger : MonoBehaviour
{
    public CurvemeterDirections Direction = CurvemeterDirections.Other;

    public delegate void DirectionTriggerHandler(CurvemeterDirections direction);
    public event DirectionTriggerHandler OnDirectionTriggered;

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        OnDirectionTriggered?.Invoke(Direction);
    }
}
