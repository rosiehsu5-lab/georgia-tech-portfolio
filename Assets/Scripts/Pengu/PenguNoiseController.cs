using UnityEngine;

// Attach to the penguin. Broadcasts noise to nearby seals when the penguin
// moves above the speed threshold or performs a noisy action (landing, sliding).
public class PenguNoiseController : MonoBehaviour
{
    [Header("Noise Settings")]
    public float speedNoiseThreshold = 3f;    // Penguin speed above which movement makes noise
    public float noiseBroadcastInterval = 0.3f; // How often to broadcast noise while moving (seconds)

    [Header("Noise Radii Per Action")]
    public float footstepNoiseRadius = 6f;
    public float jumpNoiseRadius = 10f;
    public float slideNoiseRadius = 14f;
    public float movementNoiseRadius = 8f;    // Used by speed-based polling

    [Header("Debug")]
    public bool showNoiseGizmo = true;

    [Header("Noise Meter")]
    public float noiseFalloffSpeed = 3f;  // How quickly the meter drops when quiet

    // Current normalised noise level 0-1, read by NoiseMeterUI
    public float CurrentNoiseLevel { get; private set; }

    private CharacterController controller;
    private float broadcastTimer = 0f;
    // Max radius across all actions — used to normalise the meter
    private float maxNoiseRadius;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        maxNoiseRadius = Mathf.Max(footstepNoiseRadius, jumpNoiseRadius, slideNoiseRadius, movementNoiseRadius);
    }

    void Update()
    {
        broadcastTimer -= Time.deltaTime;

        float speed = controller != null
            ? new Vector3(controller.velocity.x, 0f, controller.velocity.z).magnitude
            : 0f;

        // Continuously set meter level based on movement speed
        if (speed >= speedNoiseThreshold)
        {
            float speedNoise = Mathf.InverseLerp(speedNoiseThreshold, 20f, speed);
            float movementLevel = Mathf.InverseLerp(0f, maxNoiseRadius, movementNoiseRadius);
            SetNoiseLevel(Mathf.Max(speedNoise * movementLevel, CurrentNoiseLevel));

            if (broadcastTimer <= 0f)
            {
                BroadcastNoise(movementNoiseRadius);
                broadcastTimer = noiseBroadcastInterval;
            }
        }
        else
        {
            // Decay meter when quiet
            CurrentNoiseLevel = Mathf.MoveTowards(CurrentNoiseLevel, 0f, noiseFalloffSpeed * Time.deltaTime);
        }
    }

    // Called from PenguSoundController with a specific radius per action type
    public void MakeNoise(float radius)
    {
        SetNoiseLevel(Mathf.InverseLerp(0f, maxNoiseRadius, radius));
        BroadcastNoise(radius);
    }

    void SetNoiseLevel(float level)
    {
        CurrentNoiseLevel = Mathf.Clamp01(level);
    }

    void BroadcastNoise(float radius)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider hit in hits)
        {
            SealPatrol seal = hit.GetComponent<SealPatrol>();
            if (seal != null)
                seal.HearNoise(transform.position, radius);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!showNoiseGizmo) return;
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, footstepNoiseRadius);
        Gizmos.DrawWireSphere(transform.position, jumpNoiseRadius);
        Gizmos.DrawWireSphere(transform.position, slideNoiseRadius);
    }
}