using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [Header("Ragdoll Settings")]
    public bool startRagdoll = false;
    public float ragdollForce = 10f;
    public float ragdollDuration = 5f;
    public bool autoRecover = true;
    public float recoveryTime = 3f;
    
    [Header("Ragdoll Components")]
    public Transform[] ragdollBones;
    public Rigidbody[] ragdollRigidbodies;
    public Collider[] ragdollColliders;
    
    [Header("Original Components")]
    public Animator originalAnimator;
    public Rigidbody originalRigidbody;
    public Collider originalCollider;
    public CharacterController originalController;
    
    [Header("Impact Settings")]
    public float impactForceMultiplier = 1f;
    public float explosionRadius = 2f;
    public bool applyExplosionForce = true;
    
    private bool isRagdollActive = false;
    private Vector3 lastImpactPoint;
    private float lastImpactForce;
    private Coroutine recoveryCoroutine;
    
    void Start()
    {
        // Find all ragdoll components if not assigned
        if (ragdollBones == null || ragdollBones.Length == 0)
        {
            FindRagdollComponents();
        }
        
        // Store original components
        if (originalAnimator == null) originalAnimator = GetComponent<Animator>();
        if (originalRigidbody == null) originalRigidbody = GetComponent<Rigidbody>();
        if (originalCollider == null) originalCollider = GetComponent<Collider>();
        if (originalController == null) originalController = GetComponent<CharacterController>();
        
        // Initially disable ragdoll
        SetRagdollActive(false);
        
        // Start ragdoll if requested
        if (startRagdoll)
        {
            EnableRagdoll();
        }
    }
    
    void FindRagdollComponents()
    {
        List<Transform> bones = new List<Transform>();
        List<Rigidbody> rigidbodies = new List<Rigidbody>();
        List<Collider> colliders = new List<Collider>();
        
        // Find all child transforms that could be ragdoll bones
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child != transform) // Don't include root
            {
                bones.Add(child);
                
                Rigidbody rb = child.GetComponent<Rigidbody>();
                if (rb != null) rigidbodies.Add(rb);
                
                Collider col = child.GetComponent<Collider>();
                if (col != null) colliders.Add(col);
            }
        }
        
        ragdollBones = bones.ToArray();
        ragdollRigidbodies = rigidbodies.ToArray();
        ragdollColliders = colliders.ToArray();
    }
    
    public void EnableRagdoll()
    {
        if (isRagdollActive) return;
        
        isRagdollActive = true;
        SetRagdollActive(true);
        
        // Disable original components
        if (originalAnimator != null) originalAnimator.enabled = false;
        if (originalController != null) originalController.enabled = false;
        
        // Start recovery timer if auto-recover is enabled
        if (autoRecover && recoveryCoroutine == null)
        {
            recoveryCoroutine = StartCoroutine(RecoveryTimer());
        }
        
        Debug.Log($"{gameObject.name} ragdoll activated!");
    }
    
    public void DisableRagdoll()
    {
        if (!isRagdollActive) return;
        
        isRagdollActive = false;
        SetRagdollActive(false);
        
        // Re-enable original components
        if (originalAnimator != null) originalAnimator.enabled = true;
        if (originalController != null) originalController.enabled = true;
        
        // Stop recovery timer
        if (recoveryCoroutine != null)
        {
            StopCoroutine(recoveryCoroutine);
            recoveryCoroutine = null;
        }
        
        Debug.Log($"{gameObject.name} ragdoll deactivated!");
    }
    
    void SetRagdollActive(bool active)
    {
        // Enable/disable ragdoll rigidbodies
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            if (rb != null)
            {
                rb.isKinematic = !active;
                if (active)
                {
                    rb.useGravity = true;
                }
            }
        }
        
        // Enable/disable ragdoll colliders
        foreach (Collider col in ragdollColliders)
        {
            if (col != null)
            {
                col.enabled = active;
            }
        }
    }
    
    public void ApplyImpact(Vector3 impactPoint, Vector3 impactDirection, float force)
    {
        if (!isRagdollActive)
        {
            EnableRagdoll();
        }
        
        lastImpactPoint = impactPoint;
        lastImpactForce = force;
        
        // Apply force to all ragdoll rigidbodies
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            if (rb != null)
            {
                // Calculate direction from impact point to bone
                Vector3 forceDirection = (rb.transform.position - impactPoint).normalized;
                float distance = Vector3.Distance(rb.transform.position, impactPoint);
                
                // Apply force based on distance and impact force
                float appliedForce = force * impactForceMultiplier / (1f + distance);
                rb.AddForce(forceDirection * appliedForce, ForceMode.Impulse);
                
                // Add some random torque for more chaotic movement
                Vector3 randomTorque = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)
                ) * appliedForce * 0.1f;
                rb.AddTorque(randomTorque, ForceMode.Impulse);
            }
        }
        
        // Apply explosion force if enabled
        if (applyExplosionForce)
        {
            ApplyExplosionForce(impactPoint, force);
        }
        
        Debug.Log($"Impact applied to {gameObject.name} with force {force} at point {impactPoint}");
    }
    
    void ApplyExplosionForce(Vector3 explosionCenter, float force)
    {
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            if (rb != null)
            {
                float distance = Vector3.Distance(rb.transform.position, explosionCenter);
                if (distance < explosionRadius)
                {
                    Vector3 explosionDirection = (rb.transform.position - explosionCenter).normalized;
                    float explosionForce = force * (1f - distance / explosionRadius);
                    rb.AddExplosionForce(explosionForce, explosionCenter, explosionRadius, 0f, ForceMode.Impulse);
                }
            }
        }
    }
    
    IEnumerator RecoveryTimer()
    {
        yield return new WaitForSeconds(recoveryTime);
        
        // Check if ragdoll is still active and recover
        if (isRagdollActive)
        {
            DisableRagdoll();
        }
    }
    
    public void SetRagdollForce(float force)
    {
        ragdollForce = force;
    }
    
    public void SetImpactForceMultiplier(float multiplier)
    {
        impactForceMultiplier = multiplier;
    }
    
    public void SetExplosionRadius(float radius)
    {
        explosionRadius = radius;
    }
    
    // Public getters
    public bool IsRagdollActive() => isRagdollActive;
    public Vector3 GetLastImpactPoint() => lastImpactPoint;
    public float GetLastImpactForce() => lastImpactForce;
    
    void OnDrawGizmosSelected()
    {
        // Draw explosion radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(lastImpactPoint, explosionRadius);
        
        // Draw impact point
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(lastImpactPoint, 0.1f);
    }
}
