using UnityEngine;

public class InkProjectileLogic : MonoBehaviour
{
    [SerializeField] private GameObject inkExplosionPrefab;

    [Header("Damage")]
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float hitForceMagnitude = 5f;

    [SerializeField] private float lifetime = 5f;

    private void Start()
    {
        // Destroy the projectile after a fixed lifetime and ensure explosion occurs by invoking die()
        Invoke(nameof(die), lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Determine the hit point (use first contact if available)
        Vector3 hitPoint = transform.position;
        if (collision.contacts != null && collision.contacts.Length > 0)
            hitPoint = collision.contacts[0].point;

        // Compute a hit force from projectile position toward the hit point
        Vector3 hitDirection = (hitPoint - transform.position);
        if (hitDirection.sqrMagnitude > 0.0001f)
            hitDirection.Normalize();
        Vector3 hitForce = hitDirection * hitForceMagnitude;

        // If we collided with the player (by component or tag), apply damage using PlayerController signature
        var playerController = collision.collider.GetComponent<PlayerController>() ?? collision.collider.GetComponentInParent<PlayerController>();
        if (playerController != null)
        {
            // Use PlayerController.TakeDamage similar to EnemyController's damage application
            playerController.TakeDamage(hitPoint, hitForce, damageAmount);
        }

        // Spawn ink explosion and destroy the projectile
        if (inkExplosionPrefab != null)
            Instantiate(inkExplosionPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    private void die()
    {
        if (inkExplosionPrefab != null)
            Instantiate(inkExplosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
