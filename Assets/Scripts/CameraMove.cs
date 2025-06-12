using UnityEngine;
using System.Collections.Generic;

public class CameraMove : MonoBehaviour
{
    [Header("Target Settings")]
    public List<Transform> targets = new List<Transform>(); // List of targets to average position
    public Transform rotationTarget;                        // Single target to take Z rotation from

    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0, 10, -20);        // Offset from the average position
    public bool useZRotation = false;                       // Whether to use Z rotation from the rotation target
    public bool cameraFollowEnabled = true;                 // Toggle to completely disable the camera movement

    [Header("Spring Settings")]
    [Tooltip("Higher = stiffer spring, faster acceleration toward target")]
    public float springStrength = 50f;
    [Tooltip("Higher = more damping, less overshoot")]
    public float damping = 10f;

    private Transform camTransform;
    private Vector3 velocity;                               // Current camera velocity (for spring physics)
    private Vector3 angularVelocity;                        // For Z-axis rotation spring (optional)

    private void Start()
    {
        camTransform = Camera.main.transform;
        velocity = Vector3.zero;
        angularVelocity = Vector3.zero;
    }

    private void LateUpdate()
    {
        if (!cameraFollowEnabled) return;

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

        // 1. Compute desired position
        Vector3 averagePosition = Vector3.zero;
        foreach (Transform t in targets)
            averagePosition += t.position;
        averagePosition /= targets.Count;
        Vector3 desiredPosition = averagePosition + offset;

        // 2. Spring-damper integration for position
        //    a = k * (target - pos) - c * v
        Vector3 displacement = desiredPosition - camTransform.position;
        Vector3 accel = displacement * springStrength - velocity * damping;
        velocity += accel * Time.deltaTime;
        camTransform.position += velocity * Time.deltaTime;

        // 3. Rotation
        if (useZRotation)
        {
            // Target Z rotation (with offset so “up” points toward world up)
            float targetZ = -rotationTarget.eulerAngles.z + 90f;

            // Simple spring-damper for the Z axis (in degrees)
            float currentZ = camTransform.eulerAngles.z;
            float deltaZ = Mathf.DeltaAngle(currentZ, targetZ);
            float angAccel = deltaZ * springStrength - angularVelocity.z * damping;
            angularVelocity.z += angAccel * Time.deltaTime;
            float newZ = currentZ + angularVelocity.z * Time.deltaTime;

            camTransform.rotation = Quaternion.Euler(camTransform.eulerAngles.x,
                                                    camTransform.eulerAngles.y,
                                                    newZ);
        }
        else
        {
            // If you prefer a spring for all axes of rotation, you could replicate
            // the above for each angle. For now, we smoothly “settle” back to flat.
            Quaternion targetRot = Quaternion.Euler(20f, 0f, 0f);
            camTransform.rotation = Quaternion.Lerp(camTransform.rotation,
                                                    targetRot,
                                                    1f - Mathf.Exp(-damping * Time.deltaTime));
        }
    }
}
