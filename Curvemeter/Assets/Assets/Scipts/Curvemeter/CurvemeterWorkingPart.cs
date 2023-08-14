using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvemeterWorkingPart : MonoBehaviour
{
    public delegate void CollisionEventHandler(Collider collision);
    public event CollisionEventHandler onMapEnter;
    public event CollisionEventHandler onMapLeave;
    // Start is called before the first frame update
    // Update is called once per frame
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Map")
        {
            onMapEnter?.Invoke(collision);
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag == "Map")
        {
            onMapLeave?.Invoke(collision);
        }
    }
}
