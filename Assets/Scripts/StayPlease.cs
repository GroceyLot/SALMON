using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayPlease : MonoBehaviour
{
    public GameObject[] gameObjects;
    public Vector3 multiplier = new Vector3(1f, 1f, 0f);

    void FixedUpdate()
    {
        for (int i = 0; i < gameObjects.Length; i++)
        {
            gameObjects[i].transform.position = Vector3.Scale(gameObjects[i].transform.position, multiplier);
        }
    }
}
