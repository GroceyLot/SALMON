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

    [Header("Raycast Settings")]
    public LayerMask raycastMask; // Assign this in the Inspector to ignore the fish layer
    public float raycastDistance = 1f;

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
        GameObject mostLeft = null;
        float minX = float.MaxValue;

        // First pass: use raycast
        foreach (GameObject obj in objects)
        {
            if (obj == null) continue;

            Ray ray = new Ray(obj.transform.position, Vector3.down);
            Debug.DrawRay(obj.transform.position, Vector3.down * raycastDistance, Color.red, 1f);
            if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, raycastMask))
            {
                float x = obj.transform.position.x;
                if (x < minX)
                {
                    minX = x;
                    mostLeft = obj;
                }
            }
        }

        Debug.Log($"Most left object: {mostLeft?.name ?? "None"} at X: {minX}");
        return mostLeft;
    }

    private GameObject FindMostRightObject()
    {
        GameObject mostRight = null;
        float maxX = float.MinValue;

        // First pass: use raycast
        foreach (GameObject obj in objects)
        {
            if (obj == null) continue;

            Ray ray = new Ray(obj.transform.position, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, raycastMask))
            {
                float x = obj.transform.position.x;
                if (x > maxX)
                {
                    maxX = x;
                    mostRight = obj;
                }
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
            Vector3 opwardForce = Vector3.up * Mathf.Clamp(holdTime * forceMultiplier, 0f, maxForce);
            Vector3 horizontalForce = horizontalDirection * Mathf.Clamp(holdTime * sideForceMultiplier, 0f, maxSideForce);
            rb.AddForce(opwardForce, ForceMode.Impulse);
            rb.AddForce(horizontalForce, ForceMode.Impulse);
        }
        else
        {
            Debug.LogWarning($"GameObject '{targetObject.name}' does not have a Rigidbody component.");
        }
    }
}
