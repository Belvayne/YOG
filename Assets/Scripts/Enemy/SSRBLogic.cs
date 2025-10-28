using UnityEngine;
using System.Collections;

public class SSRBLogic : EnemyController
{
    [SerializeField] GameObject explosionEffect;
    [SerializeField] GameObject explosionPoint;

    public override void Die()
    {
        Debug.Log("SSRB initiating self-destruct sequence.");
        if (IsDead()) return;
        Debug.Log("SSRB has died.");
        base.Die(); // Handles isDead, agent cleanup, kill counter, etc.

        // Start self-destruct sequence (e.g., coroutine to explode after 3 seconds)
        StartCoroutine(SelfDestructCoroutine());
    }

    private IEnumerator SelfDestructCoroutine()
    {
        yield return new WaitForSeconds(3f);
        Debug.Log("KABOOOOM");

        // Instantiate explosion effect
        if (explosionEffect != null)
        {
            if (explosionEffect != null)
            {
                Vector3 explosionPos = GetRagdollCenter();
                Instantiate(explosionEffect, explosionPos, Quaternion.identity);
            }
        }

        // Explosion parameters
        float explosionRadius = 5f;
        float explosionForce = 2000f;
        float explosionDamage = 100f;
        Vector3 explosionPosition = transform.position;

        // Find all colliders in the explosion radius
        Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius);
        foreach (Collider hit in colliders)
        {
            // Apply damage to IDamageable entities
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null && damageable != this)
            {
                Vector3 forceDir = (hit.transform.position - explosionPosition).normalized * explosionForce;
                damageable.TakeDamage(explosionPosition, forceDir, explosionDamage);
            }

            // Apply force to rigidbodies
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, 1f, ForceMode.Impulse);
            }
        }

        //gameObject.SetActive(false);
    }

    private Vector3 GetRagdollCenter()
    {
        RagdollActivator ragdoll = GetComponent<RagdollActivator>();
        if (ragdoll != null && ragdoll.ragdollBodies != null && ragdoll.ragdollBodies.Length > 0)
        {
            Vector3 sum = Vector3.zero;
            foreach (Rigidbody rb in ragdoll.ragdollBodies)
            {
                sum += rb.worldCenterOfMass;
            }
            return (sum / ragdoll.ragdollBodies.Length) + (Vector3.up * 2.5f);
        }
        return transform.position;
    }

}