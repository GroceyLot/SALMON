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

    bool IsDescendantOrEqual(GameObject obj, GameObject target)
    {
        if (obj == null || target == null)
            return false;

        Transform current = obj.transform;

        while (current != null)
        {
            if (current.gameObject == target)
                return true;
            current = current.parent;
        }

        return false;
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
            GameObject target = checkpoints[i];
            // Add the CollisionListener component if not already present
            var listener = target.GetComponent<CollisionListener>();
            if (listener == null)
            {
                listener = target.AddComponent<CollisionListener>();
            }
            
            // Pass the callback for collision handling
            listener.OnTriggerOccurred += void (Collider other, GameObject _target) => HandleCollision(other, _target, i);
        }
    }

    void HandleCollision(Collider other, GameObject target, int index)
    {
        Debug.Log("Collision");
        if (!IsDescendantOrEqual(other, player)) {
            Debug.Log("with something");
            return;
        }
        Debug.Log("with player");
        saveData.data.checkpoint = index;
    }
}