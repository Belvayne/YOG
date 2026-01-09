using UnityEngine;
using UnityEngine.AI;

public class TakodachiLogic : EnemyController
{
    // Flying/hovering
    [SerializeField] private float maxHeight = 3f; // maximum distance allowed above the nearest surface below
    [SerializeField] private float minHeight = 1f; // minimum distance allowed above the nearest surface below
    [SerializeField] private float currentHeight = 0f; // updated at runtime

    // Movement
    [SerializeField] private float speed = 5f;
    private NavMeshAgent agent;

    // Vertical control
    [SerializeField] private float bobFrequency = 0.5f; // cycles per second
    [SerializeField] private float verticalSmoothSpeed = 5f; // smoothing speed for lerp
    private float bobPhase;

    // (left for potential future use)
    [SerializeField] private float floatTargetThreshold = 0.05f; // legacy

    //attack
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float lastAttackTime = -Mathf.Infinity;
    [SerializeField] private float attackRange = 50f;
    [SerializeField] private GameObject projectilePrefab;

    private Transform playerTransform;

    public override void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = GetComponent<NavMeshAgent>();

        // Prevent NavMeshAgent from directly setting transform position/rotation so we control vertical movement
        if (agent != null)
        {
            agent.updatePosition = false;
            agent.updateRotation = false;
            agent.speed = speed;
        }

        // randomize bob phase so multiple takodachi don't bob in unison
        bobPhase = Random.Range(0f, Mathf.PI * 2f);
    }

    public override void Update()
    {
        // Fix: Check if playerController is not null and use PlayerController's IsDead() method
        if (isDead || playerTransform == null)
            return;

        var playerController = playerTransform.GetComponent<PlayerController>();
        if (playerController != null && playerController.IsDead())
            return;

        // Let NavMeshAgent compute a horizontal path, but do not let it move us vertically
        if (agent != null)
            agent.SetDestination(playerTransform.position);

        // Determine horizontal target: prefer agent steering target so the enemy navigates around obstacles
        Vector3 horizontalTarget;
        if (agent != null && agent.hasPath)
        {
            var steer = agent.steeringTarget;
            horizontalTarget = new Vector3(steer.x, transform.position.y, steer.z);
        }
        else
        {
            horizontalTarget = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
        }

        // Move horizontally toward target
        Vector3 newPos = Vector3.MoveTowards(transform.position, horizontalTarget, speed * Time.deltaTime);

        // Vertical control: raycast down to find surface under the enemy and compute min/max allowed Y
        RaycastHit hit;
        float maxSearchDistance = 100f;
        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, maxSearchDistance))
        {
            currentHeight = hit.distance - 0.1f; // account for small offset

            float minAllowedY = hit.point.y + minHeight;
            float maxAllowedY = hit.point.y + maxHeight;

            // Sine-wave bobbing between min and max
            float centerY = (minAllowedY + maxAllowedY) * 0.5f;
            float amplitude = Mathf.Max(0.001f, (maxAllowedY - minAllowedY) * 0.5f);
            float desiredY = centerY + amplitude * Mathf.Sin(Time.time * (Mathf.PI * 2f * bobFrequency) + bobPhase);

            // Clamp desiredY to ensure it stays within bounds
            desiredY = Mathf.Clamp(desiredY, minAllowedY, maxAllowedY);

            // Smoothly move the transform's Y toward desiredY to avoid jitter
            float smoothY = Mathf.Lerp(transform.position.y, desiredY, Time.deltaTime * verticalSmoothSpeed);
            newPos.y = smoothY;
        }
        else
        {
            // No surface found below within search distance: keep current Y
            currentHeight = float.PositiveInfinity;
            newPos.y = transform.position.y;
        }

        // Apply horizontal movement and smoothed vertical separately
        transform.position = newPos;

        // Keep agent internal position in sync with our transform so pathfinding remains stable
        if (agent != null)
            agent.nextPosition = new Vector3(transform.position.x, agent.nextPosition.y, transform.position.z);

        // Face the player (yaw only)
        Vector3 lookDir = playerTransform.position - transform.position;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
        }

        // Attack-only logic: deal damage when within range and cooldown passed
        float sqrDistance = (playerTransform.position - transform.position).sqrMagnitude;
        float attackRangeSqr = attackRange * attackRange;
        if (sqrDistance <= attackRangeSqr && Time.time >= lastAttackTime + attackCooldown)
        {
            //trigger projectile attack here
            ShootProjectile();
            lastAttackTime = Time.time;
        }
    }

    private void ShootProjectile()
    {
        Debug.Log("Takodachi: Shooting projectile at player.");
        if (projectilePrefab == null)
        {
            Debug.LogWarning("Takodachi: projectilePrefab is not assigned.");
            return;
        }

        if (playerTransform == null)
        {
            Debug.LogWarning("Takodachi: playerTransform is null.");
            return;
        }

        // Spawn slightly in front of the enemy
        Vector3 spawnPos = transform.position + transform.forward * 1f;

        // Aim direction toward player's current position
        Vector3 aimDir = (playerTransform.position - spawnPos).normalized;
        Quaternion rot = Quaternion.LookRotation(aimDir);

        GameObject proj = Instantiate(projectilePrefab, spawnPos, rot);

        // Try to get player's movement velocity (prefer CharacterController.velocity, fallback to Rigidbody)
        Vector3 playerVelocity = Vector3.zero;
        var playerCC = playerTransform.GetComponent<CharacterController>();
        if (playerCC != null)
        {
            playerVelocity = playerCC.velocity;
        }
        else
        {
            var playerRb = playerTransform.GetComponent<Rigidbody>();
            if (playerRb != null)
                playerVelocity = playerRb.linearVelocity;
        }

        // Set projectile velocity: forward * speed + player's velocity to account for movement
        const float projectileSpeed = 5f;
        var rb = proj.GetComponent<Rigidbody>();
        if (rb == null)
        {
            // If prefab lacks Rigidbody, add one so we can set velocity. Disable gravity for projectiles by default.
            rb = proj.AddComponent<Rigidbody>();
            rb.useGravity = false;
        }

        rb.linearVelocity = aimDir * projectileSpeed + playerVelocity;
    }
}
