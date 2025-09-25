using UnityEngine;

public class SimpleEnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    
    [Header("Target")]
    public Transform target;
    public string targetTag = "Player";
    
    [Header("Behavior")]
    public bool canAttack = true;
    public bool alwaysChase = false;
    public bool useGravity = true;
    
    [Header("Visual")]
    public GameObject detectionIndicator;
    public GameObject attackIndicator;
    public float indicatorLifetime = 0.5f;
    
    private Rigidbody rb;
    private Animator animator;
    private float lastAttackTime;
    private bool isChasing = false;
    private bool isAttacking = false;
    private bool targetDetected = false;
    private bool isDead = false;
    
    void Start()
    {
        // Get components
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        
        // Find target if not assigned
        if (target == null)
        {
            FindTarget();
        }
        
        // Setup Rigidbody
        if (rb != null)
        {
            rb.useGravity = useGravity;
            rb.freezeRotation = true; // Prevent physics rotation
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
        
        // Update behavior
        if (targetDetected || alwaysChase)
        {
            if (!isChasing)
            {
                StartChasing();
            }
            ChaseTarget();
        }
        
        // Update animations
        UpdateAnimations();
    }
    
    void FindTarget()
    {
        GameObject targetObj = GameObject.FindGameObjectWithTag(targetTag);
        if (targetObj != null)
        {
            target = targetObj.transform;
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
        
        Debug.Log($"{gameObject.name} started chasing!");
    }
    
    void OnTargetLost()
    {
        isChasing = false;
        isAttacking = false;
        
        // Stop movement
        if (rb != null)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
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
            MoveTowardsTarget();
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
            Vector3 newVelocity = new Vector3(direction.x * moveSpeed, rb.linearVelocity.y, direction.z * moveSpeed);
            rb.linearVelocity = newVelocity;
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
        if (rb != null)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
        
        // Show attack indicator
        if (attackIndicator != null)
        {
            GameObject indicator = Instantiate(attackIndicator, transform.position + Vector3.up * 2f, Quaternion.identity);
            Destroy(indicator, indicatorLifetime);
        }
        
        // Perform attack
        PerformAttack();
        
        Debug.Log($"{gameObject.name} attacked!");
        
        // Reset attack state
        Invoke(nameof(ResetAttack), 0.5f);
    }
    
    void PerformAttack()
    {
        // Try to damage player if close enough
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance <= attackRange)
            {
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
    
    void UpdateAnimations()
    {
        if (animator == null) return;
        
        // Set animation parameters
        animator.SetBool("IsWalking", isChasing && !isAttacking);
        animator.SetBool("IsChasing", isChasing);
        animator.SetBool("IsAttacking", isAttacking);
        
        // Set speed parameter
        float currentSpeed = isChasing ? moveSpeed : 0f;
        animator.SetFloat("Speed", currentSpeed);
    }
    
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }
    
    public void SetDetectionRange(float range)
    {
        detectionRange = range;
    }
    
    public void SetAttackRange(float range)
    {
        attackRange = range;
    }
    
    // Public getters
    public bool IsChasing() => isChasing;
    public bool IsAttacking() => isAttacking;
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
    }
}
