using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, IDamageable
{
    [SerializeField] private float currentHealth = 100f;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float rotationSpeed = 5f;

    [SerializeField] private float hitForceMagnitude = 5f;

    public bool isDead = false;
    private float lastAttackTime = -Mathf.Infinity;
    private NavMeshAgent agent;

    public virtual void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = GetComponent<NavMeshAgent>();
    }

    public virtual void Update()
    {
        // Fix: Check if playerController is not null and use PlayerController's IsDead() method
        if (isDead || playerTransform == null)
            return;

        var playerController = playerTransform.GetComponent<PlayerController>();
        if (playerController != null && playerController.IsDead())
            return;

        // NavMeshAgent handles movement
        if (agent != null)
            agent.destination = playerTransform.position;

        // Attack-only logic: deal damage when within range and cooldown passed
        float sqrDistance = (playerTransform.position - transform.position).sqrMagnitude;
        float attackRangeSqr = attackRange * attackRange;
        if (sqrDistance <= attackRangeSqr && Time.time >= lastAttackTime + attackCooldown)
        {
            if (playerController != null && playerController.GetCurrentHealth() > 0)
            {
                // Determine closest point on the player's collider (fallback to player position)
                Vector3 hitPoint = playerTransform.position;
                Collider playerCollider = playerTransform.GetComponent<Collider>();
                if (playerCollider != null)
                {
                    hitPoint = playerCollider.ClosestPoint(transform.position);
                }
                else
                {
                    // If no Collider found, try CharacterController (use its transform as fallback)
                    var cc = playerTransform.GetComponent<CharacterController>();
                    if (cc != null)
                        hitPoint = cc.transform.position;
                }

                // Compute a small hit force pushing from enemy toward the player hit point
                Vector3 hitDirection = (hitPoint - transform.position);
                if (hitDirection.sqrMagnitude > 0.0001f)
                    hitDirection.Normalize();
                Vector3 hitForce = hitDirection * hitForceMagnitude;

                // Use the PlayerController's signature that accepts hitPoint and hitForce
                playerController.TakeDamage(hitPoint, hitForce, attackDamage);
            }
            lastAttackTime = Time.time;
        }
    }

    public void TakeDamage(Vector3 hitPoint, Vector3 hitForce, float damage)
    {
        if (isDead) return;
        Debug.Log($"{gameObject.name} took {damage} damage.");
        currentHealth -= damage;
        if (currentHealth <= 0f)
        {
            Debug.Log($"{gameObject.name} has died.");
            Die();
            var ragdoll = GetComponent<RagdollActivator>();
            if (ragdoll != null)
                ragdoll.ActivateRagdoll(hitPoint, hitForce);
        }
    }

    public virtual void Die()
    {
        if (isDead) return;

        isDead = true;
        Destroy(agent);
        
        // Notify LevelManager of the kill
        var levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.OnEnemyKilled();
        }
    }

    public bool IsDead() => isDead;
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}