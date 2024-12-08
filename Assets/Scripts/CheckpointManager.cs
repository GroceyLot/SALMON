using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public SaveData saveData;
    public GameObject[] checkpoints;
    public GameObject player; // The object to be moved to the checkpoint position on start.

    void Start()
    {
        StartCoroutine(LateStart());
    }
    IEnumerator LateStart()
    {
        yield return null;
        // Move the player to the saved checkpoint position on start.
        if (saveData != null && saveData.data != null && checkpoints.Length > 0)
        {
            int checkpointIndex = saveData.data.checkpoint;

            if (checkpointIndex >= 0 && checkpointIndex < checkpoints.Length)
            {
                player.transform.position = checkpoints[checkpointIndex].transform.position;
            }
        }

        for (int i = 0; i < checkpoints.Length; i++)
        {
            // Add the CollisionListener component if not already present
            var listener = target.GetComponent<CollisionListener>();
            if (listener == null)
            {
                listener = target.AddComponent<CollisionListener>();
            }
            
            // Pass the callback for collision handling
            listener.OnTriggerOccurred += void (Collider other, GameObject target) => HandleCollision(other, target, i);
        }
    }

    void HandleCollision(Collider other, GameObject target, int index)
    {
        saveData.data.checkpoint = index;
    }
}