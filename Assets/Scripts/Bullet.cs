using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float damage = 10f;
    public float lifetime = 5f;
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
    
    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        
        // Check if we should hit this object
        if (((1 << other.gameObject.layer) & hitLayers) == 0)
            return;
        
        hasHit = true;
        
        // Apply damage if target has health component
        Health targetHealth = other.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
        }
        
        // Create hit effect
        if (hitEffect != null)
        {
            GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.LookRotation(transform.forward));
            Destroy(effect, 2f);
        }
        
        // Stop the bullet
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }
        
        // Destroy bullet
        Destroy(gameObject);
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;
        
        // Check if we should hit this object
        if (((1 << collision.gameObject.layer) & hitLayers) == 0)
            return;
        
        hasHit = true;
        
        // Apply damage if target has health component
        Health targetHealth = collision.gameObject.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
        }
        
        // Create hit effect
        if (hitEffect != null)
        {
            ContactPoint contact = collision.contacts[0];
            GameObject effect = Instantiate(hitEffect, contact.point, Quaternion.LookRotation(contact.normal));
            Destroy(effect, 2f);
        }
        
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
