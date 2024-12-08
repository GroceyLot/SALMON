using UnityEngine;

public class SlopSound : MonoBehaviour
{
    [Header("Object Settings")]
    public AudioSource audioSource; // Assign the AudioSource in the inspector
    public AudioClip[] slopSounds; // Array of "slop" sounds
    public GameObject[] targetObjects; // Array of GameObjects to monitor for collisions
    [Header("Collision Settings")]
    public float minVelocity = 1f;  // Minimum collision velocity to trigger the sound
    public float maxVelocity = 10f; // Maximum velocity for max volume
    [Header("Sound Settings")]
    public float minVolume = 0.1f;  // Minimum volume
    public float maxVolume = 1f;    // Maximum volume
    public float volumeModifier = 0.1f; // Volume modifier
    public float pitchRange = 0.1f; // Pitch range

    void Start()
    {
        // Attach CollisionListener to each target object
        foreach (GameObject target in targetObjects)
        {
            if (target != null)
            {
                // Add the CollisionListener component if not already present
                var listener = target.GetComponent<CollisionListener>();
                if (listener == null)
                {
                    listener = target.AddComponent<CollisionListener>();
                }

                // Pass the callback for collision handling
                listener.OnCollisionOccurred += HandleCollision;
            }
        }
    }

    // Callback method to handle collisions
    void HandleCollision(Collision collision, GameObject target)
    {
        // Get the relative velocity magnitude
        float impactForce = collision.relativeVelocity.magnitude;

        // Play the sound only if the impact force is above the minimum threshold
        if (impactForce > minVelocity && slopSounds.Length > 0)
        {
            // Choose a random sound from the array
            AudioClip randomClip = slopSounds[Random.Range(0, slopSounds.Length)];

            // Map the impact force to a volume level
            float volume = Mathf.Clamp(
                Mathf.InverseLerp(minVelocity, maxVelocity, impactForce),
                minVolume,
                maxVolume
            ) + Random.Range(-volumeModifier, volumeModifier);
            audioSource.pitch = 1f + Random.Range(-pitchRange, pitchRange);

            // Play the sound with the randomized pitch
            audioSource.PlayOneShot(randomClip, volume);

            // Reset pitch to original value
            audioSource.pitch = 1f;
        }
    }
}