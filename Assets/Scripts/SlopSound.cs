using UnityEngine;
using System.Linq;                  // for Sum()
using Unity.VisualScripting;      // for your CollisionListener hook

public class SlopSound : MonoBehaviour
{
    [Header("Object Settings")]
    public AudioSource audioSource;     // Assign in inspector
    public AudioClip[] slopSounds;      // Array of "slop" sounds
    public GameObject[] targetObjects;  // Objects to monitor

    [Header("Collision Settings")]
    public float minVelocity = 1f;      // Min velocity to trigger
    public float maxVelocity = 10f;     // Velocity for full volume

    [Header("Sound Settings")]
    public float minVolume = 0.1f;      // Minimum volume clamp
    public float maxVolume = 1f;        // Maximum volume clamp
    public float volumeModifier = 0.1f; // Random volume tweak
    public float pitchRange = 0.1f;     // Random pitch tweak

    [Header("Filtering & Cooldown")]
    public GameObject[] ignoreObjects;  // Things to skip
    public float cooldown = 0.1f;       // Seconds between sounds

    void Start()
    {
        // Attach CollisionListener to each target
        foreach (GameObject target in targetObjects)
        {
            if (target != null)
            {
                var listener = target.GetComponent<CollisionListener>();
                if (listener == null)
                    listener = target.AddComponent<CollisionListener>();
                listener.OnCollisionOccurred += HandleCollision;
            }
        }
    }

    void Update()
    {
        // Tick down our cooldown timer
        cooldown -= Time.deltaTime;
    }

    // Called by your CollisionListener
    void HandleCollision(Collision collision, GameObject target)
    {
        // Skip hits on ignored or other targets
        if (ignoreObjects.Contains(collision.collider.gameObject) ||
            targetObjects.Contains(collision.collider.gameObject))
        {
            return;
        }

        float impactForce = collision.relativeVelocity.magnitude;
        if (impactForce > minVelocity && slopSounds.Length > 0 && cooldown <= 0f)
        {
            // 1) Normalize force into 0–1
            float norm = Mathf.Clamp01(
                Mathf.InverseLerp(minVelocity, maxVelocity, impactForce)
            );

            // 2) Build weights so later clips get boosted by 'norm'
            int N = slopSounds.Length;
            float[] weights = new float[N];
            for (int i = 0; i < N; i++)
            {
                // base weight 1, plus up to +1 when norm=1 and i=N−1
                weights[i] = 1f + norm * (i / (float)(N - 1));
            }

            // 3) Pick a clip index by weighted random
            int idx = GetWeightedRandomIndex(weights);
            AudioClip chosenClip = slopSounds[idx];

            // 4) Compute volume & pitch, then play
            float volume = Mathf.Lerp(minVolume, maxVolume, norm)
                           + Random.Range(-volumeModifier, volumeModifier);
            audioSource.pitch = 1f + Random.Range(-pitchRange, pitchRange);
            audioSource.PlayOneShot(chosenClip, volume);

            // Reset pitch & cooldown
            audioSource.pitch = 1f;
            cooldown = 0.1f;
        }
    }

    // Standard weighted‐random helper
    int GetWeightedRandomIndex(float[] weights)
    {
        float total = weights.Sum();
        float r = Random.Range(0f, total);
        float accum = 0f;

        for (int i = 0; i < weights.Length; i++)
        {
            accum += weights[i];
            if (r <= accum)
                return i;
        }

        return weights.Length - 1; // fallback
    }
}
