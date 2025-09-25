using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrazyPhysicsController : MonoBehaviour
{
    [Header("Crazy Physics Settings")]
    public float baseForceMultiplier = 1f;
    public float explosionForce = 15f;
    public float explosionRadius = 3f;
    public float upwardForce = 5f;
    public float spinForce = 10f;
    public float bounceForce = 8f;
    
    [Header("Force Types")]
    public bool useExplosionForce = true;
    public bool useUpwardForce = true;
    public bool useSpinForce = true;
    public bool useBounceForce = true;
    public bool useRandomForces = true;
    
    [Header("Random Force Settings")]
    public float randomForceMin = 5f;
    public float randomForceMax = 15f;
    public int randomForceCount = 3;
    public float randomForceRadius = 2f;
    
    [Header("Visual Effects")]
    public ParticleSystem impactEffect;
    public GameObject explosionPrefab;
    public float effectLifetime = 2f;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] impactSounds;
    public AudioClip explosionSound;
    public float volume = 1f;
    
    [Header("Physics Materials")]
    public PhysicsMaterial bouncyMaterial;
    public PhysicsMaterial slipperyMaterial;
    
    private Rigidbody rb;
    private Collider col;
    private bool hasBeenHit = false;
    private Vector3 lastImpactPoint;
    private float lastImpactForce;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        
        // Set up physics materials for crazy bouncing
        if (bouncyMaterial != null && col != null)
        {
            col.material = bouncyMaterial;
        }
        
        // Ensure we have an audio source
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }
    
    public void ApplyCrazyPhysics(Vector3 impactPoint, Vector3 impactDirection, float impactForce)
    {
        if (rb == null) return;
        
        lastImpactPoint = impactPoint;
        lastImpactForce = impactForce;
        hasBeenHit = true;
        
        Debug.Log($"Applying crazy physics to {gameObject.name} with force {impactForce}!");
        
        // Apply different types of forces
        if (useExplosionForce)
        {
            ApplyExplosionForce(impactPoint, impactForce);
        }
        
        if (useUpwardForce)
        {
            ApplyUpwardForce(impactForce);
        }
        
        if (useSpinForce)
        {
            ApplySpinForce(impactDirection, impactForce);
        }
        
        if (useBounceForce)
        {
            ApplyBounceForce(impactDirection, impactForce);
        }
        
        if (useRandomForces)
        {
            ApplyRandomForces(impactPoint, impactForce);
        }
        
        // Play effects
        PlayImpactEffects(impactPoint);
        
        // Start coroutine for additional effects
        StartCoroutine(CrazyPhysicsSequence(impactForce));
    }
    
    void ApplyExplosionForce(Vector3 explosionCenter, float force)
    {
        rb.AddExplosionForce(
            explosionForce * baseForceMultiplier * force * 0.1f,
            explosionCenter,
            explosionRadius,
            upwardForce,
            ForceMode.Impulse
        );
    }
    
    void ApplyUpwardForce(float force)
    {
        Vector3 upwardDirection = Vector3.up + Random.insideUnitSphere * 0.3f;
        rb.AddForce(upwardDirection * upwardForce * baseForceMultiplier * force * 0.1f, ForceMode.Impulse);
    }
    
    void ApplySpinForce(Vector3 direction, float force)
    {
        Vector3 randomAxis = Random.insideUnitSphere;
        Vector3 spinDirection = Vector3.Cross(direction, randomAxis).normalized;
        rb.AddTorque(spinDirection * spinForce * baseForceMultiplier * force * 0.1f, ForceMode.Impulse);
    }
    
    void ApplyBounceForce(Vector3 direction, float force)
    {
        Vector3 bounceDirection = Vector3.Reflect(direction, Vector3.up);
        bounceDirection += Random.insideUnitSphere * 0.5f;
        bounceDirection.Normalize();
        
        rb.AddForce(bounceDirection * bounceForce * baseForceMultiplier * force * 0.1f, ForceMode.Impulse);
    }
    
    void ApplyRandomForces(Vector3 center, float force)
    {
        for (int i = 0; i < randomForceCount; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * randomForceRadius;
            Vector3 randomDirection = (randomPoint - center).normalized;
            float randomForce = Random.Range(randomForceMin, randomForceMax);
            
            rb.AddForceAtPosition(
                randomDirection * randomForce * baseForceMultiplier * force * 0.1f,
                randomPoint,
                ForceMode.Impulse
            );
        }
    }
    
    void PlayImpactEffects(Vector3 impactPoint)
    {
        // Play impact particle effect
        if (impactEffect != null)
        {
            impactEffect.transform.position = impactPoint;
            impactEffect.Play();
        }
        
        // Spawn explosion effect
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, impactPoint, Quaternion.identity);
            Destroy(explosion, effectLifetime);
        }
        
        // Play random impact sound
        if (audioSource != null && impactSounds.Length > 0)
        {
            AudioClip randomSound = impactSounds[Random.Range(0, impactSounds.Length)];
            audioSource.PlayOneShot(randomSound, volume);
        }
        
        // Play explosion sound
        if (audioSource != null && explosionSound != null)
        {
            audioSource.PlayOneShot(explosionSound, volume * 0.7f);
        }
    }
    
    IEnumerator CrazyPhysicsSequence(float impactForce)
    {
        // Apply additional forces over time for sustained crazy physics
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(0.1f);
            
            if (rb != null)
            {
                // Apply random impulses
                Vector3 randomForce = Random.insideUnitSphere * impactForce * 0.05f;
                rb.AddForce(randomForce, ForceMode.Impulse);
                
                // Apply random torque
                Vector3 randomTorque = Random.insideUnitSphere * impactForce * 0.02f;
                rb.AddTorque(randomTorque, ForceMode.Impulse);
            }
        }
    }
    
    public void SetForceMultiplier(float multiplier)
    {
        baseForceMultiplier = multiplier;
    }
    
    public void SetExplosionSettings(float force, float radius)
    {
        explosionForce = force;
        explosionRadius = radius;
    }
    
    public void SetBounceSettings(float force)
    {
        bounceForce = force;
    }
    
    public void SetSpinSettings(float force)
    {
        spinForce = force;
    }
    
    // Public getters
    public bool HasBeenHit() => hasBeenHit;
    public Vector3 GetLastImpactPoint() => lastImpactPoint;
    public float GetLastImpactForce() => lastImpactForce;
    
    void OnDrawGizmosSelected()
    {
        // Draw explosion radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(lastImpactPoint, explosionRadius);
        
        // Draw impact point
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(lastImpactPoint, 0.2f);
        
        // Draw random force radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(lastImpactPoint, randomForceRadius);
    }
}
