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
        StartCoroutine(ExplosionCoroutine());
        StartCoroutine(SelfDestructCoroutine());
    }

    private IEnumerator ExplosionCoroutine()
    {
        yield return new WaitForSeconds(2.8f);
        Vector3 explosionPos = GetRagdollCenter();

        // Instantiate explosion effect
        if (explosionEffect != null)
        {
            if (explosionEffect != null)
            {
                Instantiate(explosionEffect, explosionPos, Quaternion.identity);
            }
        }
    }

    private IEnumerator SelfDestructCoroutine()
    {
        yield return new WaitForSeconds(3f);
        Vector3 explosionPos = GetRagdollCenter();

        // Explosion parameters
        float explosionRadius = 2.5f;
        float explosionForce = 100f;
        float explosionDamage = 100f;

        // Find all colliders in the explosion radius
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
        foreach (Collider hit in colliders)
        {
            // Apply damage to IDamageable entities
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null && damageable != this)
            {
                Vector3 forceDir = (hit.transform.position - explosionPos).normalized * explosionForce;
                damageable.TakeDamage(explosionPos, forceDir, explosionDamage);
            }

            PlayerController playerController = hit.GetComponent<PlayerController>();
            if (playerController != null)
            {
                Vector3 forceDir = (hit.transform.position - explosionPos).normalized * explosionForce;
                playerController.TakeDamage(explosionPos, forceDir, explosionDamage);
            }

            // Apply force to rigidbodies
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, explosionPos, explosionRadius, 1f, ForceMode.Impulse);
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
            return (sum / ragdoll.ragdollBodies.Length);
        }
        return transform.position;
    }

}