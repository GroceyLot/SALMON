using UnityEngine;

public class MoveScript : MonoBehaviour
{
    [Header("Object Settings")]
    public GameObject[] objects;
    private float holdTimeLeft = 0f;
    private float holdTimeRight = 0f;

    [Header("Force Settings")]
    public float maxForce = 100f;
    public float maxSideForce = 50f;
    private float sideForceMultiplier = 0f;
    private float forceMultiplier = 0f;

    [Header("UI Settings")]
    public GameObject UIObject;
    public float fillTime = 0f;

    [Header("Air Jump Settings")]
    public float airJumpForce = 10f;              // Upward force when doing an air jump
    public float airJumpCooldown = 2f;            // Cooldown in seconds
    private float airJumpCooldownTimer = 0f;      // Timer tracking cooldown

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
        }

        // Update cooldown timer
        if (airJumpCooldownTimer > 0f)
        {
            airJumpCooldownTimer -= Time.deltaTime;
        }
    }

    private void HandleInput()
    {
        // Left mouse button
        if (Input.GetMouseButton(0))
        {
            holdTimeLeft += Time.deltaTime;
        }

        if (Input.GetMouseButtonUp(0))
        {
            GameObject mostLeftObject = FindMostLeftObject();
            ApplyForce(mostLeftObject, holdTimeLeft, Vector3.right);
            holdTimeLeft = 0f;
        }

        // Right mouse button
        if (Input.GetMouseButton(1))
        {
            holdTimeRight += Time.deltaTime;
        }

        if (Input.GetMouseButtonUp(1))
        {
            GameObject mostRightObject = FindMostRightObject();
            ApplyForce(mostRightObject, holdTimeRight, Vector3.left);
            holdTimeRight = 0f;
        }

        // Middle mouse button (Air Jump)
        if (Input.GetMouseButtonDown(2) && airJumpCooldownTimer <= 0f)
        {
            foreach (GameObject obj in objects)
            {
                if (obj == null) continue;
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(Vector3.up * airJumpForce, ForceMode.Impulse);
                }
            }
            airJumpCooldownTimer = airJumpCooldown;
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

    private void ApplyForce(GameObject targetObject, float holdTime, Vector3 horizontalDirection)
    {
        if (targetObject == null) return;

        Rigidbody rb = targetObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 downwardForce = Vector3.down * Mathf.Clamp(holdTime * forceMultiplier, 0f, maxForce);
            Vector3 horizontalForce = horizontalDirection * Mathf.Clamp(holdTime * sideForceMultiplier, 0f, maxSideForce);
            rb.AddForce(downwardForce, ForceMode.Impulse);
            rb.AddForce(horizontalForce, ForceMode.Impulse);
        }
        else
        {
            Debug.LogWarning($"GameObject '{targetObject.name}' does not have a Rigidbody component.");
        }
    }
}
