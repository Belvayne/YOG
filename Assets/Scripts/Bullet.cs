using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float damage = 10f;
    public float lifetime = 5f;
    public float bulletSpeed = 20f;
    public LayerMask hitLayers = ~0;
    public GameObject hitEffect;
    

    [Header("Physics")]
    public bool useGravity = false;
    public float bounceForce = 0f;
    
    private Rigidbody rb;
    private bool hasHit = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Set gravity
        if (rb != null)
        {
            rb.useGravity = useGravity;
        }
        
        // Auto-destroy after lifetime
        Destroy(gameObject, lifetime);
    }
    
    //void OnTriggerEnter(Collider other)
    //{
    //    if (hasHit) return;
        
    //    // Check if we should hit this object
    //    if (((1 << other.gameObject.layer) & hitLayers) == 0)
    //        return;
        
    //    hasHit = true;
        
    //    // Get impact point and direction
    //    Vector3 impactPoint = transform.position;
    //    Vector3 impactDirection = transform.forward;
    //    float impactForce = rb != null ? rb.linearVelocity.magnitude : bulletSpeed;
        
    //    // Apply damage if target has health component
    //    Health targetHealth = other.GetComponent<Health>();
    //    if (targetHealth != null)
    //    {
    //        targetHealth.TakeDamage(damage);
    //    }
        
    //    // Apply damage and impact to enemy health
    //    EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
    //    if (enemyHealth != null)
    //    {
    //        enemyHealth.TakeDamage(damage, impactPoint, impactDirection, impactForce);
    //    }
        
    //    // Apply damage and impact to simple enemy health
    //    SimpleEnemyHealth simpleEnemyHealth = other.GetComponent<SimpleEnemyHealth>();
    //    if (simpleEnemyHealth != null)
    //    {
    //        simpleEnemyHealth.TakeDamage(damage, impactPoint, impactDirection, impactForce);
    //    }
        
    //    // Apply impact force to ragdoll if present
    //    RagdollController ragdoll = other.GetComponent<RagdollController>();
    //    if (ragdoll != null)
    //    {
    //        ragdoll.ApplyImpact(impactPoint, impactDirection, impactForce);
    //    }
        
    //    // Apply crazy physics if present
    //    CrazyPhysicsController crazyPhysics = other.GetComponent<CrazyPhysicsController>();
    //    if (crazyPhysics != null)
    //    {
    //        crazyPhysics.ApplyCrazyPhysics(impactPoint, impactDirection, impactForce);
    //    }
        
    //    // Create hit effect
    //    if (hitEffect != null)
    //    {
    //        GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.LookRotation(transform.forward));
    //        Destroy(effect, 2f);
    //    }
        
    //    // Stop the bullet
    //    if (rb != null)
    //    {
    //        rb.linearVelocity = Vector3.zero;
    //    }
        
    //    // Destroy bullet
    //    Destroy(gameObject);
    //}
    
    void OnCollisionEnter(Collision collision)
    {
        var enemyController = collision.collider.GetComponentInParent<EnemyController>();
        if (enemyController != null)
        {
            Vector3 hitPos = collision.contacts[0].point;
            Vector3 hitDir = transform.forward; // or collision.relativeVelocity.normalized
            float forcePower = 100f; // tweak for comedic effect
            Debug.Log($"Bullet hit enemy at {hitPos} with direction {hitDir}, dealing {damage} damage");
            enemyController.TakeDamage(hitPos, hitDir * forcePower, damage);
            Destroy(gameObject); // destroy bullet
        }

        //var ragdollPhysics = collision.collider.GetComponentInParent<RagdollActivator>();
        //if (ragdollPhysics != null)
        //{
        //    Vector3 hitPos = collision.contacts[0].point;
        //    Vector3 hitDir = transform.forward; // or collision.relativeVelocity.normalized
        //    float forcePower = 100f; // tweak for comedic effect

        //    ragdollPhysics.ActivateRagdoll(hitPos, hitDir * forcePower);
        //    Destroy(gameObject); // destroy bullet
        //}

        //if (hasHit) return;
        
        //// Check if we should hit this object
        //if (((1 << collision.gameObject.layer) & hitLayers) == 0)
        //    return;
        
        //hasHit = true;
        
        //// Get impact point and direction from collision
        //ContactPoint contact = collision.contacts[0];
        //Vector3 impactPoint = contact.point;
        //Vector3 impactDirection = transform.forward;
        //float impactForce = rb != null ? rb.linearVelocity.magnitude : bulletSpeed;
        
        //// Apply damage if target has health component
        //Health targetHealth = collision.gameObject.GetComponent<Health>();
        //if (targetHealth != null)
        //{
        //    targetHealth.TakeDamage(damage);
        //}
        
        //// Apply damage and impact to enemy health
        //EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
        //if (enemyHealth != null)
        //{
        //    enemyHealth.TakeDamage(damage, impactPoint, impactDirection, impactForce);
        //}
        
        //// Apply damage and impact to simple enemy health
        //SimpleEnemyHealth simpleEnemyHealth = collision.gameObject.GetComponent<SimpleEnemyHealth>();
        //if (simpleEnemyHealth != null)
        //{
        //    simpleEnemyHealth.TakeDamage(damage, impactPoint, impactDirection, impactForce);
        //}
        
        //// Apply impact force to ragdoll if present
        //RagdollController ragdoll = collision.gameObject.GetComponent<RagdollController>();
        //if (ragdoll != null)
        //{
        //    ragdoll.ApplyImpact(impactPoint, impactDirection, impactForce);
        //}
        
        //// Apply crazy physics if present
        //CrazyPhysicsController crazyPhysics = collision.gameObject.GetComponent<CrazyPhysicsController>();
        //if (crazyPhysics != null)
        //{
        //    crazyPhysics.ApplyCrazyPhysics(impactPoint, impactDirection, impactForce);
        //}
        
        //// Create hit effect
        //if (hitEffect != null)
        //{
        //    GameObject effect = Instantiate(hitEffect, contact.point, Quaternion.LookRotation(contact.normal));
        //    Destroy(effect, 2f);
        //}
        
        // Handle bouncing
        if (bounceForce > 0f && rb != null)
        {
            Vector3 bounceDirection = Vector3.Reflect(transform.forward, collision.contacts[0].normal);
            rb.linearVelocity = bounceDirection * bounceForce;
            hasHit = false; // Allow for multiple bounces
        }
        else
        {
            // Stop the bullet
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
            }
            
            // Destroy bullet
            Destroy(gameObject);
        }
    }
}
