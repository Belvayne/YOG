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

    private bool isDead = false;
    private float lastAttackTime = -Mathf.Infinity;
    private NavMeshAgent agent;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (isDead || playerTransform == null) return;
        agent.destination = playerTransform.position;
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