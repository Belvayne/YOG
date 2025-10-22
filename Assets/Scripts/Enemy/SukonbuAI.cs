using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float currentHealth = 100f;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float moveSpeed = 2.5f;
    //[SerializeField] private float detectionRange = 15f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private KillCounter killCounter;
    private bool isDead = false;

    private float lastAttackTime = -Mathf.Infinity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        killCounter = GameObject.FindGameObjectWithTag("KillCounter")?.GetComponent<KillCounter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            if (playerTransform == null) return;
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            agent.destination = playerTransform.position;
            //float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            ////if (distanceToPlayer <= detectionRange)
            ////{
            //// Move towards the player if not in attack range
            //if (distanceToPlayer > attackRange)
            //{
            //    Vector3 direction = (playerTransform.position - transform.position).normalized;
            //    Rigidbody rb = GetComponent<Rigidbody>();
            //    if (rb != null)
            //    {
            //        Vector3 move = direction * moveSpeed;
            //        move.y = rb.linearVelocity.y; // Preserve gravity
            //        Debug.Log($"Enemy moving towards player with velocity {move}");
            //        rb.MovePosition(rb.position + move * Time.deltaTime);
            //    }

            //    Quaternion targetRotation = Quaternion.LookRotation(direction);
            //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            //}
        }
    }

    private void AttackPlayer()
    {
        // Assumes the player has a TakeDamage(float amount) method
        var player = playerTransform.GetComponent<PlayerController>();
        if (player != null)
        {
            //player.TakeDamage(attackDamage);
        }
    }

    // Call this method when the enemy takes damage
    public void TakeDamage(Vector3 hitPoint, Vector3 hitForce, float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0f)
        {
            isDead = true;
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            Destroy(agent);
            var ragdoll = GetComponent<RagdollActivator>();
            if (ragdoll != null)
            {
                ragdoll.ActivateRagdoll(hitPoint, hitForce);
            }
            killCounter.AddKill();
        }
    }
}
