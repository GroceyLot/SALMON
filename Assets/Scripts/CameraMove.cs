using UnityEngine;
using System.Collections.Generic;

public class CameraMove : MonoBehaviour
{
    [Header("Target Settings")]
    public List<Transform> targets = new List<Transform>(); // List of targets to average position
    public Transform rotationTarget; // Single target to take Z rotation from

    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0, 10, -20); // Offset from the average position
    public float smoothing = 0.5f; // Smoothing factor for position and rotation
    public bool useZRotation = false; // Whether to use Z rotation from the rotation target

    private void LateUpdate()
    {
        if (targets == null || targets.Count == 0)
        {
            Debug.LogWarning("CameraMove: No position targets assigned.");
            return;
        }

        if (useZRotation && rotationTarget == null)
        {
            Debug.LogWarning("CameraMove: Z rotation is enabled, but no rotation target assigned.");
            return;
        }

        // Average the positions of all targets
        Vector3 averagePosition = Vector3.zero;
        foreach (Transform t in targets)
        {
            averagePosition += t.position;
        }
        averagePosition /= targets.Count;

        // Desired camera position with offset
        Vector3 desiredPosition = averagePosition + offset;

        // Smooth camera movement
        transform.position = Vector3.Lerp(transform.position, desiredPosition, (1f / smoothing) * Time.deltaTime);

        // Conditionally apply Z rotation
        if (useZRotation)
        {
            Vector3 currentEuler = transform.eulerAngles;
            float targetZ = -rotationTarget.eulerAngles.z + 90f;
            float smoothedZ = Mathf.LerpAngle(currentEuler.z, targetZ, (1f / smoothing) * Time.deltaTime);
            transform.rotation = Quaternion.Euler(currentEuler.x, currentEuler.y, smoothedZ);
        }
    }
}
