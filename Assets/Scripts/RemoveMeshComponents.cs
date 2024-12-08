using UnityEngine;

public class RemoveMeshComponents : MonoBehaviour
{
    public string targetTag = "YourTagName";
    public bool enabled = true;

    void Start()
    {
        if (!enabled) return;
        // Find all objects with the specified tag
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(targetTag);

        foreach (GameObject obj in taggedObjects)
        {
            // Remove MeshRenderer if it exists
            MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                Destroy(meshRenderer);
            }

            // Remove MeshFilter if it exists
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                Destroy(meshFilter);
            }
        }

        Debug.Log($"Removed MeshRenderer and MeshFilter from all objects with tag '{targetTag}'.");
    }
}