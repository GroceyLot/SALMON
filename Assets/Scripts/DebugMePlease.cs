using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMePlease : MonoBehaviour
{
    public Collider? collider;
    void Start() 
    {

    }

    void Update()
    {
        if (collider != null)
        {
            Debug.Log("Collider Size: " + collider.bounds.size);
        }
    }
}
