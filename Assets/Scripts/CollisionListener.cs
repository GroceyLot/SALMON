using UnityEngine;

// Helper class to listen for collisions
public class CollisionListener : MonoBehaviour
{
    public delegate void CollisionEventHandler(Collision collision, GameObject target);
    public event CollisionEventHandler OnCollisionOccurred;
    public delegate void TriggerEventHandler(Collider other, GameObject target);
    public event TriggerEventHandler OnTriggerOccurred;

    void OnCollisionEnter(Collision collision)
    {
        // Invoke the event if a subscriber exists
        OnCollisionOccurred?.Invoke(collision, gameObject);
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Invoke the event if a subscriber exists
        OnTriggerOccurred?.Invoke(other, gameObject);
    }
}