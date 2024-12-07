using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyTransform : MonoBehaviour
{
    public GameObject[] targets;
    public GameObject[] gameObjects;

    void LateUpdate()
    {
        for (int i = 0; i < gameObjects.Length; i++) {
            gameObjects[i].transform.position = targets[i].transform.position;
            gameObjects[i].transform.rotation = targets[i].transform.rotation;
        }
    }
}
