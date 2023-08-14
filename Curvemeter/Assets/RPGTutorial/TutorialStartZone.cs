using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialStartZone : MonoBehaviour
{
    Canvas _dialogRenderer;
    MeshRenderer _zoneRenderer;

    private void Awake()
    {
        _dialogRenderer = transform.parent.GetComponentInChildren<Canvas>();
        _zoneRenderer = transform.GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent != null
            && other.transform.parent.name.Contains("Hand")
            && _zoneRenderer.enabled
            && !RPGTutorial.TutorialIsGoing)
        {
            _dialogRenderer.enabled = true;
            _zoneRenderer.enabled = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent != null 
            && other.transform.parent.name.Contains("Hand")
            && !_zoneRenderer.enabled
            && !RPGTutorial.TutorialIsGoing)
        {
            _dialogRenderer.enabled = false;
            _zoneRenderer.enabled = true;
        }
    }
}
