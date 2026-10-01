using UnityEngine;
using UnityEngine.AI;

public class SealPatrol : MonoBehaviour
{
    [Header("Patrol")]
    public Transform[] waypoints;
    public float waypointTolerance = 0.5f;
    public float patrolSpeed = 2.5f;

    [Header("Investigation")]
    public float noiseDetectionRadius = 10f;
    public float investigateSpeed = 4f;
    public float investigateWaitTime = 2f;

    [Header("Proximity Detection")]
    public float proximityDetectionRadius = 5f;  // Triggers investigate if penguin enters this radius

    [Header("Chase")]
    public Transform player;
    public Rigidbody playerRigidbody;
    public float chaseSpeed = 10f;

    [Header("Line of Sight")]
    public float detectionRange = 10f;
    [Range(0f, 360f)] public float fieldOfViewAngle = 160f;
    public Transform eyePoint;
    public LayerMask obstructionMask = ~0;

    [Header("Chase Memory")]
    public float loseSightDelay = 4f;

    [Header("Jump Interception")]
    [Tooltip("How far above the seal the player must be to trigger landing prediction")]
    public float jumpDetectionHeight = 0.4f;

    [Header("Audio")]
    public AudioSource alertAudioSource;

    [Header("Animation")]
    public Animator sealAnimator;

    private PenguController penguController;
    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;
    private float loseSightTimer = 0f;
    private Vector3 lastKnownPlayerPosition;
    private Vector3 investigateTarget;
    private float investigateTimer = 0f;

    private enum State { Patrol, Investigate, Chase }
    private State currentState = State.Patrol;
    private State previousState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("SealPatrol: No NavMeshAgent found.");
            enabled = false;
            return;
        }

        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("SealPatrol: No waypoints assigned.");
            enabled = false;
            return;
        }

        if (player == null)
        {
            Debug.LogError("SealPatrol: No player assigned.");
            enabled = false;
            return;
        }

        if (playerRigidbody == null)
        {
            Debug.LogError("SealPatrol: No player Rigidbody assigned.");
            enabled = false;
            return;
        }

        penguController = player.GetComponent<PenguController>();

        previousState = currentState;
        agent.speed = patrolSpeed;
        agent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    void Update()
    {
        // While Pengu is invincible after respawn, seal loses interest and returns to patrol
        if (penguController != null && penguController.IsInvincible)
        {
            if (currentState == State.Chase)
            {
                currentState = State.Patrol;
                agent.SetDestination(waypoints[currentWaypointIndex].position);
            }
            return;
        }

        bool canSeePlayer = CanSeePlayer();

        // --- Proximity check ---
        // Triggers investigate (not chase) so the seal notices but doesn't immediately aggro.
        // Only fires when not already chasing so it doesn't interrupt a chase.
        if (currentState != State.Chase)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= proximityDetectionRadius)
            {
                investigateTarget = player.position;
                investigateTimer = investigateWaitTime;
                currentState = State.Investigate;
            }
        }

        // When player is airborne directly above (jumped over), treat as still visible for chase purposes
        bool playerAirborneOverhead = currentState == State.Chase
            && (player.position.y - transform.position.y) > jumpDetectionHeight;

        // Chase always overrides other states if player is visible
        if (canSeePlayer || playerAirborneOverhead)
        {
            currentState = State.Chase;
            loseSightTimer = loseSightDelay;
            if (canSeePlayer) lastKnownPlayerPosition = player.position;
        }
        else if (currentState == State.Chase)
        {
            loseSightTimer -= Time.deltaTime;

            if (loseSightTimer <= 0f)
            {
                currentState = State.Patrol;
                agent.SetDestination(waypoints[currentWaypointIndex].position);
            }
        }

        switch (currentState)
        {
            case State.Patrol: Patrol(); break;
            case State.Investigate: Investigate(); break;
            case State.Chase: Chase(); break;
        }

        if (currentState != previousState)
        {
            if (currentState == State.Chase && alertAudioSource != null)
                alertAudioSource.Play();

            // Stop barking when seal leaves Chase state
            if (previousState == State.Chase && currentState != State.Chase && alertAudioSource != null)
                alertAudioSource.Stop();

            Debug.Log("State changed to: " + currentState);
            previousState = currentState;
        }

        if (sealAnimator != null)
        {
            // Drive animation directly from state rather than agent.velocity
            // which can be unreliable depending on NavMesh configuration
            switch (currentState)
            {
                case State.Patrol:
                    sealAnimator.SetFloat("Speed", patrolSpeed);
                    break;
                case State.Investigate:
                    sealAnimator.SetFloat("Speed", investigateSpeed);
                    break;
                case State.Chase:
                    sealAnimator.SetFloat("Speed", chaseSpeed);
                    break;
            }
        }
    }

    void Patrol()
    {
        agent.speed = patrolSpeed;
        if (agent.pathPending) return;

        if (agent.remainingDistance <= waypointTolerance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    void Investigate()
    {
        agent.speed = investigateSpeed;
        agent.SetDestination(investigateTarget);

        if (!agent.pathPending && agent.remainingDistance <= waypointTolerance)
        {
            investigateTimer -= Time.deltaTime;

            if (investigateTimer <= 0f)
            {
                currentState = State.Patrol;
                agent.SetDestination(waypoints[currentWaypointIndex].position);
            }
        }
    }

    void Chase()
    {
        agent.speed = chaseSpeed;

        Vector3 playerVelocity = playerRigidbody.linearVelocity;
        float heightAbove = player.position.y - transform.position.y;
        bool playerIsAirborne = heightAbove > jumpDetectionHeight;

        // When the player is jumping over the seal, keep tracking them and intercept landing
        if (playerIsAirborne)
        {
            lastKnownPlayerPosition = player.position;

            // Estimate time to land using kinematic equations: y = vy*t + 0.5*g*t^2
            float groundY = agent.nextPosition.y;
            float relativeHeight = player.position.y - groundY;
            float vy = playerVelocity.y;
            float g = Physics.gravity.y; // negative

            float discriminant = vy * vy - 2f * g * relativeHeight;
            float landingTime = (discriminant >= 0f) ? (-vy - Mathf.Sqrt(discriminant)) / g : 0f;
            landingTime = Mathf.Max(0f, landingTime);

            Vector3 landingPos = new Vector3(
                player.position.x + playerVelocity.x * landingTime,
                groundY,
                player.position.z + playerVelocity.z * landingTime
            );
            agent.SetDestination(landingPos);
        }
        else if (CanSeePlayer())
        {
            lastKnownPlayerPosition = player.position;
            // Predict where player will be ~0.5s ahead
            Vector3 predicted = player.position + playerVelocity * 0.5f;
            agent.SetDestination(predicted);
        }
        else
        {
            agent.SetDestination(lastKnownPlayerPosition);
        }
    }

    // Called by PenguNoise � radius is the noise's reach, passed in per action type
    public void HearNoise(Vector3 noisePosition, float radius)
    {
        if (currentState == State.Chase) return;

        float distanceToNoise = Vector3.Distance(transform.position, noisePosition);

        if (distanceToNoise <= radius)
        {
            investigateTarget = noisePosition;
            investigateTimer = investigateWaitTime;
            currentState = State.Investigate;

            Debug.Log("Seal heard noise and is investigating.");
        }
    }

    bool CanSeePlayer()
    {
        Vector3 origin = eyePoint != null ? eyePoint.position : transform.position + Vector3.up * 0.3f;
        Vector3 directionToPlayer = player.position - origin;

        float distanceToPlayer = directionToPlayer.magnitude;
        if (distanceToPlayer > detectionRange) return false;

        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer > fieldOfViewAngle * 0.5f) return false;

        if (Physics.Raycast(origin, directionToPlayer.normalized, out RaycastHit hit, detectionRange, obstructionMask))
        {
            if (hit.transform == player || hit.transform.IsChildOf(player))
                return true;
        }

        return false;
    }

    void OnDrawGizmosSelected()
    {
        Vector3 origin = eyePoint != null ? eyePoint.position : transform.position + Vector3.up * 0.3f;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, detectionRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, proximityDetectionRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(origin, origin + transform.forward * detectionRange);

        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfViewAngle * 0.5f, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfViewAngle * 0.5f, 0) * transform.forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origin, origin + leftBoundary * detectionRange);
        Gizmos.DrawLine(origin, origin + rightBoundary * detectionRange);

        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, player.position);
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, noiseDetectionRadius);
    }
}