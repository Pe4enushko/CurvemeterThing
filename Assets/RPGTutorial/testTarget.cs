using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testTarget : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        GetComponent<MeshRenderer>().material.color = Color.red;
        print(collision.transform.position);

        Invoke(nameof(RestoreColor), 1.5f);
    }
    void RestoreColor()
    {
        GetComponent<MeshRenderer>().material.color = Color.white;
    }
}
