using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("AI Settings")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public float wanderRadius = 5f;
    public float wanderTimer = 3f;
    
    [Header("Behavior")]
    public bool useNavMesh = true;
    public bool canAttack = true;
    public bool canWander = true;
    public bool alwaysChase = false;
    
    [Header("Target")]
    public Transform target;
    public string targetTag = "Player";
    public LayerMask targetLayer = ~0;
    
    [Header("Visual")]
    public GameObject detectionIndicator;
    public GameObject attackIndicator;
    public float indicatorLifetime = 0.5f;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] attackSounds;
    public AudioClip[] detectionSounds;
    public float audioVolume = 1f;
    
    private NavMeshAgent navAgent;
    private Rigidbody rb;
    private Animator animator;
    private Vector3 startPosition;
    private Vector3 wanderTarget;
    private float lastAttackTime;
    private float wanderTime;
    private bool isChasing = false;
    private bool isAttacking = false;
    private bool isWandering = false;
    private bool targetDetected = false;
    
    void Start()
    {
        // Get components
        navAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        
        // Store start position for wandering
        startPosition = transform.position;
        
        // Setup NavMesh agent
        if (navAgent != null)
        {
            navAgent.speed = moveSpeed;
            navAgent.angularSpeed = rotationSpeed * 10f;
            navAgent.stoppingDistance = attackRange;
        }
        
        // Find target if not assigned
        if (target == null)
        {
            FindTarget();
        }
        
        // Setup audio source
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        // Start wandering if enabled
        if (canWander && !alwaysChase)
        {
            StartWandering();
        }
    }
    
    void Update()
    {
        if (target == null)
        {
            FindTarget();
            return;
        }
        
        // Check for target detection
        CheckTargetDetection();
        
        // Update behavior based on state
        if (targetDetected || alwaysChase)
        {
            if (!isChasing)
            {
                StartChasing();
            }
            ChaseTarget();
        }
        else if (canWander && !isChasing)
        {
            Wander();
        }
        
        // Update animations
        UpdateAnimations();
    }
    
    void FindTarget()
    {
        // Find target by tag
        GameObject targetObj = GameObject.FindGameObjectWithTag(targetTag);
        if (targetObj != null)
        {
            target = targetObj.transform;
            return;
        }
        
        // Find target by layer
        Collider[] targets = Physics.OverlapSphere(transform.position, detectionRange, targetLayer);
        if (targets.Length > 0)
        {
            target = targets[0].transform;
        }
    }
    
    void CheckTargetDetection()
    {
        if (target == null) return;
        
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        
        if (distanceToTarget <= detectionRange)
        {
            if (!targetDetected)
            {
                targetDetected = true;
                OnTargetDetected();
            }
        }
        else
        {
            if (targetDetected && !alwaysChase)
            {
                targetDetected = false;
                OnTargetLost();
            }
        }
    }
    
    void OnTargetDetected()
    {
        isChasing = true;
        isWandering = false;
        
        // Play detection sound
        if (audioSource != null && detectionSounds.Length > 0)
        {
            AudioClip randomSound = detectionSounds[Random.Range(0, detectionSounds.Length)];
            audioSource.PlayOneShot(randomSound, audioVolume);
        }
        
        // Show detection indicator
        if (detectionIndicator != null)
        {
            GameObject indicator = Instantiate(detectionIndicator, transform.position + Vector3.up * 2f, Quaternion.identity);
            Destroy(indicator, indicatorLifetime);
        }
        
        Debug.Log($"{gameObject.name} detected target!");
    }
    
    void StartChasing()
    {
        isChasing = true;
        isWandering = false;
        
        // Stop any current wandering
        if (navAgent != null)
        {
            navAgent.ResetPath();
        }
        
        Debug.Log($"{gameObject.name} started chasing!");
    }
    
    void OnTargetLost()
    {
        isChasing = false;
        isAttacking = false;
        
        // Stop NavMesh agent
        if (navAgent != null)
        {
            navAgent.ResetPath();
        }
        
        Debug.Log($"{gameObject.name} lost target!");
    }
    
    void ChaseTarget()
    {
        if (target == null) return;
        
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        
        // Check if in attack range
        if (distanceToTarget <= attackRange && canAttack)
        {
            if (!isAttacking && Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
            }
        }
        else
        {
            // Move towards target
            if (useNavMesh && navAgent != null)
            {
                navAgent.SetDestination(target.position);
            }
            else
            {
                MoveTowardsTarget();
            }
        }
    }
    
    void MoveTowardsTarget()
    {
        if (target == null) return;
        
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f; // Keep movement on horizontal plane
        
        // Move using Rigidbody
        if (rb != null)
        {
            rb.linearVelocity = new Vector3(direction.x * moveSpeed, rb.linearVelocity.y, direction.z * moveSpeed);
        }
        else
        {
            // Move using Transform
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
        
        // Rotate towards target
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    
    void Attack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        
        // Stop movement
        if (navAgent != null)
        {
            navAgent.ResetPath();
        }
        
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }
        
        // Play attack sound
        if (audioSource != null && attackSounds.Length > 0)
        {
            AudioClip randomSound = attackSounds[Random.Range(0, attackSounds.Length)];
            audioSource.PlayOneShot(randomSound, audioVolume);
        }
        
        // Show attack indicator
        if (attackIndicator != null)
        {
            GameObject indicator = Instantiate(attackIndicator, transform.position + Vector3.up * 2f, Quaternion.identity);
            Destroy(indicator, indicatorLifetime);
        }
        
        // Perform attack (you can customize this)
        PerformAttack();
        
        Debug.Log($"{gameObject.name} attacked!");
        
        // Reset attack state
        Invoke(nameof(ResetAttack), 0.5f);
    }
    
    void PerformAttack()
    {
        // This is where you can add attack logic
        // For example: damage player, spawn projectiles, etc.
        
        // Example: Damage player if close enough
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance <= attackRange)
            {
                // Try to damage player
                Health playerHealth = target.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(10f);
                }
            }
        }
    }
    
    void ResetAttack()
    {
        isAttacking = false;
    }
    
    void Wander()
    {
        if (!canWander) return;
        
        wanderTime += Time.deltaTime;
        
        if (wanderTime >= wanderTimer || !isWandering)
        {
            // Choose new wander target
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection.y = 0f;
            wanderTarget = startPosition + randomDirection;
            
            wanderTime = 0f;
            isWandering = true;
        }
        
        // Move towards wander target
        if (useNavMesh && navAgent != null)
        {
            navAgent.SetDestination(wanderTarget);
        }
        else
        {
            Vector3 direction = (wanderTarget - transform.position).normalized;
            direction.y = 0f;
            
            if (rb != null)
            {
                rb.linearVelocity = new Vector3(direction.x * moveSpeed * 0.5f, rb.linearVelocity.y, direction.z * moveSpeed * 0.5f);
            }
            else
            {
                transform.position += direction * moveSpeed * 0.5f * Time.deltaTime;
            }
            
            // Rotate towards wander target
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * 0.5f * Time.deltaTime);
            }
        }
    }
    
    void StartWandering()
    {
        isWandering = true;
        wanderTime = 0f;
    }
    
    void UpdateAnimations()
    {
        if (animator == null) return;
        
        // Set animation parameters
        animator.SetBool("IsWalking", isChasing || isWandering);
        animator.SetBool("IsChasing", isChasing);
        animator.SetBool("IsAttacking", isAttacking);
        
        // Set speed parameter
        float currentSpeed = 0f;
        if (isChasing)
        {
            currentSpeed = moveSpeed;
        }
        else if (isWandering)
        {
            currentSpeed = moveSpeed * 0.5f;
        }
        
        animator.SetFloat("Speed", currentSpeed);
    }
    
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
        if (navAgent != null)
        {
            navAgent.speed = speed;
        }
    }
    
    public void SetDetectionRange(float range)
    {
        detectionRange = range;
    }
    
    public void SetAttackRange(float range)
    {
        attackRange = range;
        if (navAgent != null)
        {
            navAgent.stoppingDistance = range;
        }
    }
    
    // Public getters
    public bool IsChasing() => isChasing;
    public bool IsAttacking() => isAttacking;
    public bool IsWandering() => isWandering;
    public bool IsTargetDetected() => targetDetected;
    public Transform GetTarget() => target;
    
    void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Draw wander radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(startPosition, wanderRadius);
        
        // Draw wander target
        if (isWandering)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(wanderTarget, 0.5f);
            Gizmos.DrawLine(transform.position, wanderTarget);
        }
    }
}
