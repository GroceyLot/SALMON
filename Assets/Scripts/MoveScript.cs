using UnityEngine;

public class MoveScript : MonoBehaviour
{
    [Header("Object Settings")]
    public GameObject[] objects; // Array of GameObjects
    private float holdTimeLeft = 0f; // Tracks how long left mouse button is held
    private float holdTimeRight = 0f; // Tracks how long right mouse button is held
    [Header("Force Settings")]
    public float maxForce = 100f; // Maximum force applied
    public float maxSideForce = 50f; // Maximum force applied to the sides
    public float middleForce = 75f; // Force applied when middle mouse button is pressed
    public float middleCooldownTime = 5f; // Cooldown time for middle mouse button
    private float middleCooldown = 0f; // Cooldown timer for middle mouse button
    private float sideForceMultiplier = 0f; // Multiplier for the side force
    private float forceMultiplier = 0f;
    [Header("UI Settings")]
    public GameObject UIObject;
    public float fillTime = 0f;

    void Start()
    {
        forceMultiplier = maxForce / fillTime;
        sideForceMultiplier = maxSideForce / fillTime;
    }

    void Update()
    {
        HandleInput();
        if (UIObject != null)
        {
            UIObject.GetComponent<UIScript>().leftValue = holdTimeLeft / fillTime;
            UIObject.GetComponent<UIScript>().rightValue = holdTimeRight / fillTime;
            UIObject.GetComponent<UIScript>().middleValue = middleCooldown;
        }
        if (middleCooldown > 0)
        {
            middleCooldown -= Time.deltaTime / middleCooldownTime;
        }
    }

    private void HandleInput()
    {
        // Left mouse button
        if (Input.GetMouseButton(0))
        {
            holdTimeLeft += Time.deltaTime; // Increment timer while button is held
        }

        if (Input.GetMouseButtonUp(0))
        {
            GameObject mostLeftObject = FindMostLeftObject();
            ApplyForce(mostLeftObject, holdTimeLeft, Vector3.right); // Apply force to the most left object
            holdTimeLeft = 0f; // Reset timer after button is released
        }

        // Right mouse button
        if (Input.GetMouseButton(1))
        {
            holdTimeRight += Time.deltaTime; // Increment timer while button is held
        }

        if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
        {
            holdTimeLeft = 0f;
            holdTimeRight = 0f;
            if (middleCooldown < 0.01f)
            {
                GameObject middleObject = FindMiddleObject();
                ApplyMiddleForce(middleObject);
                middleCooldown = 1;
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            GameObject mostRightObject = FindMostRightObject();
            ApplyForce(mostRightObject, holdTimeRight, Vector3.left); // Apply force to the most right object
            holdTimeRight = 0f; // Reset timer after button is released
        }

        // Middle mouse button
        if (Input.GetMouseButton(2) && middleCooldown < 0.01f)
        {
            GameObject middleObject = FindMiddleObject();
            ApplyMiddleForce(middleObject); // Apply immediate force to the middle object
            middleCooldown = 1;
        }
    }

    private GameObject FindMostLeftObject()
    {
        if (objects == null || objects.Length == 0) return null;

        GameObject mostLeft = objects[0];
        foreach (GameObject obj in objects)
        {
            if (obj != null && obj.transform.position.x < mostLeft.transform.position.x)
            {
                mostLeft = obj;
            }
        }
        return mostLeft;
    }

    private GameObject FindMostRightObject()
    {
        if (objects == null || objects.Length == 0) return null;

        GameObject mostRight = objects[0];
        foreach (GameObject obj in objects)
        {
            if (obj != null && obj.transform.position.x > mostRight.transform.position.x)
            {
                mostRight = obj;
            }
        }
        return mostRight;
    }

    private GameObject FindMiddleObject()
    {
        if (objects == null || objects.Length == 0) return null;

        GameObject middleObject = objects[0];
        float closestToCenter = Mathf.Abs(objects[0].transform.position.x);

        foreach (GameObject obj in objects)
        {
            if (obj != null)
            {
                float distanceToCenter = Mathf.Abs(obj.transform.position.x);
                if (distanceToCenter < closestToCenter)
                {
                    closestToCenter = distanceToCenter;
                    middleObject = obj;
                }
            }
        }
        return middleObject;
    }

    private void ApplyForce(GameObject targetObject, float holdTime, Vector3 horizontalDirection)
    {
        if (targetObject == null) return; // Ensure object exists

        Rigidbody rb = targetObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Calculate and apply combined force
            Vector3 downwardForce = Vector3.down * Mathf.Clamp(holdTime * forceMultiplier, 0f, maxForce);
            Vector3 horizontalForce = horizontalDirection * Mathf.Clamp(holdTime * sideForceMultiplier, 0f, maxSideForce);
            rb.AddForce(downwardForce + horizontalForce, ForceMode.Impulse);
        }
        else
        {
            Debug.LogWarning($"GameObject '{targetObject.name}' does not have a Rigidbody component.");
        }
    }

    private void ApplyMiddleForce(GameObject targetObject)
    {
        if (targetObject == null) return; // Ensure object exists

        Rigidbody rb = targetObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 upwardForce = Vector3.down * middleForce; // Apply upward force
            rb.AddForce(upwardForce, ForceMode.Impulse);
        }
        else
        {
            Debug.LogWarning($"GameObject '{targetObject.name}' does not have a Rigidbody component.");
        }
    }
}
