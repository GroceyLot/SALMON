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
    public float raycastDistance = 0.25f;

    private bool isHoldingLeft = false;
    private bool isHoldingRight = false;

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
        // ── LEFT (Mouse0 or A) ────────────────────────────────────────────────
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.A))
        {
            isHoldingLeft = true;
            holdTimeLeft = 0f;
        }

        if (isHoldingLeft)
            holdTimeLeft += Time.deltaTime;

        if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.A))
        {
            if (isHoldingLeft)
            {
                (GameObject mostLeftObject, bool wallJump) = FindMostLeftObject();
                ApplyForce(mostLeftObject, holdTimeLeft, Vector3.right, wallJump);
                holdTimeLeft = 0f;
            }
            isHoldingLeft = false;
        }

        // ── RIGHT (Mouse1 or D) ───────────────────────────────────────────────
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.D))
        {
            isHoldingRight = true;
            holdTimeRight = 0f;
        }

        if (isHoldingRight)
            holdTimeRight += Time.deltaTime;

        if (Input.GetMouseButtonUp(1) || Input.GetKeyUp(KeyCode.D))
        {
            if (isHoldingRight)
            {
                (GameObject mostRightObject, bool wallJump) = FindMostRightObject();
                ApplyForce(mostRightObject, holdTimeRight, Vector3.left, wallJump);
                holdTimeRight = 0f;
            }
            isHoldingRight = false;
        }

        // ── AIR JUMP (Mouse2 or W) ────────────────────────────────────────────
        if ((Input.GetMouseButtonDown(2) || Input.GetKeyDown(KeyCode.W))
             && airJumpCooldownTimer <= 0f)
        {
            foreach (GameObject obj in objects)
            {
                if (obj == null) continue;
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null)
                    rb.AddForce(Vector3.up * airJumpForce, ForceMode.Impulse);
            }
            airJumpCooldownTimer = airJumpCooldown;
        }
    }

    // Helper: casts the object’s BoxCollider (with its rotation) in a world-space direction
    private bool IsTouchingGlobalDirection(BoxCollider box, Vector3 worldDir)
    {
        Vector3 center = box.bounds.center;
        Vector3 halfExtents = box.size * 0.5f * 0.95f;      // slight shrink to avoid self-hits
        Quaternion orient = box.transform.rotation;

        return Physics.BoxCast(
            center,
            halfExtents,
            worldDir,
            out _,
            orient,
            raycastDistance,
            raycastMask
        );
    }

    private (GameObject, bool) FindMostLeftObject()
    {
        GameObject mostLeft = null;
        float minX = float.MaxValue;
        bool anyGround = false;

        // 1) Look for objects “standing” on something below
        foreach (GameObject obj in objects)
        {
            if (obj == null) continue;
            var box = obj.GetComponent<BoxCollider>();
            if (box == null) continue;

            if (IsTouchingGlobalDirection(box, Vector3.down))
            {
                float x = obj.transform.position.x;
                if (x < minX)
                {
                    minX = x;
                    mostLeft = obj;
                    anyGround = true;
                }
            }
        }

        if (anyGround)
            return (mostLeft, false);

        // 2) No ground hits → look for a wall to the left
        foreach (GameObject obj in objects)
        {
            if (obj == null) continue;
            var box = obj.GetComponent<BoxCollider>();
            if (box == null) continue;

            if (IsTouchingGlobalDirection(box, Vector3.left))
            {
                float x = obj.transform.position.x;
                if (x < minX)
                {
                    minX = x;
                    mostLeft = obj;
                }
            }
        }

        return (mostLeft, true);
    }

    private (GameObject, bool) FindMostRightObject()
    {
        GameObject mostRight = null;
        float maxX = float.MinValue;
        bool anyGround = false;

        // 1) Look for objects “standing” on something below
        foreach (GameObject obj in objects)
        {
            if (obj == null) continue;
            var box = obj.GetComponent<BoxCollider>();
            if (box == null) continue;

            if (IsTouchingGlobalDirection(box, Vector3.down))
            {
                float x = obj.transform.position.x;
                if (x > maxX)
                {
                    maxX = x;
                    mostRight = obj;
                    anyGround = true;
                }
            }
        }

        if (anyGround)
            return (mostRight, false);

        // 2) No ground hits → look for a wall to the right
        foreach (GameObject obj in objects)
        {
            if (obj == null) continue;
            var box = obj.GetComponent<BoxCollider>();
            if (box == null) continue;

            if (IsTouchingGlobalDirection(box, Vector3.right))
            {
                float x = obj.transform.position.x;
                if (x > maxX)
                {
                    maxX = x;
                    mostRight = obj;
                }
            }
        }

        return (mostRight, true);
    }

    private void ApplyForce(GameObject targetObject, float holdTime, Vector3 horizontalDirection, bool wallJump = false)
    {
        if (targetObject == null) return;

        Rigidbody rb = targetObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            float opwardMag = Mathf.Clamp(holdTime * forceMultiplier, 0f, maxForce);
            float horizontalMag = Mathf.Clamp(holdTime * sideForceMultiplier, 0f, maxSideForce);
            Vector3 opwardForce = Vector3.up * (wallJump ? horizontalMag : opwardMag);
            Vector3 horizontalForce = horizontalDirection * (wallJump ? opwardMag : horizontalMag);
            rb.AddForce(opwardForce, ForceMode.Impulse);
            rb.AddForce(horizontalForce, ForceMode.Impulse);
        }
        else
        {
            Debug.LogWarning($"GameObject '{targetObject.name}' does not have a Rigidbody component.");
        }
    }
}
