using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // The target for the camera to follow

    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0, 10, -20); // Offset from the target
    public float smoothing = 100f; // Smoothing factor for the follow

    private void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("SmoothCameraFollow: No target assigned to the camera.");
            return;
        }

        // Calculate the desired position
        Vector3 desiredPosition = target.position + offset;

        // Smoothly interpolate to the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, (1f/smoothing) * Time.deltaTime);

        // Update the camera position
        transform.position = smoothedPosition;
    }
}
