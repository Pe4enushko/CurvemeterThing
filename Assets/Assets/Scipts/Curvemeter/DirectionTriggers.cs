using Assets.Assets.Scipts.Curvemeter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionTriggers : MonoBehaviour
{
    float _detectDistance = 0.03f;

    public delegate void DirectionTriggerHandler(CurvemeterDirections direction);
    public event DirectionTriggerHandler OnDirectionTriggered;

    public Transform Curvemeter;

    public AnyDirectionTrigger Forth;
    public AnyDirectionTrigger Back;
    public AnyDirectionTrigger Left;
    public AnyDirectionTrigger Right;
    // Start is called before the first frame update
    void Start()
    {
        Forth.OnDirectionTriggered += Send;
        Back.OnDirectionTriggered += Send;
        Left.OnDirectionTriggered += Send;
        Right.OnDirectionTriggered += Send;
    }

    void Send(CurvemeterDirections direction)
    {
        OnDirectionTriggered.Invoke(direction);

        ResetTriggers();
    }

    void ResetTriggers()
    {
        var targetPosition = Curvemeter.localPosition;
        transform.localPosition = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z);
    }
}
