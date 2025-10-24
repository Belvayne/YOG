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
    
    void OnCollisionEnter(Collision collision)
    {
        IDamageable damageable = collision.collider.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            Vector3 hitPos = collision.contacts[0].point;
            Vector3 hitDir = transform.forward;
            float forcePower = 100f;
            damageable.TakeDamage(hitPos, hitDir * forcePower, damage);
            Destroy(gameObject);
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
