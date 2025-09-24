using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public bool isInvulnerable = false;
    public bool dieOnZeroHealth = true;
    
    [Header("Ragdoll Integration")]
    public RagdollController ragdollController;
    public bool enableRagdollOnDamage = true;
    public bool enableRagdollOnDeath = true;
    public float ragdollForceOnDeath = 15f;
    
    [Header("Death Settings")]
    public float deathDelay = 0f;
    public bool destroyOnDeath = true;
    public float destroyDelay = 5f;
    
    [Header("Events")]
    public UnityEvent<float> OnHealthChanged;
    public UnityEvent OnDeath;
    public UnityEvent<float> OnDamageTaken;
    public UnityEvent OnRagdollActivated;
    
    private float currentHealth;
    private bool isDead = false;
    private bool ragdollActivated = false;
    
    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);
        
        // Get ragdoll controller if not assigned
        if (ragdollController == null)
        {
            ragdollController = GetComponent<RagdollController>();
        }
    }
    
    public void TakeDamage(float damage, Vector3 impactPoint = default, Vector3 impactDirection = default, float impactForce = 0f)
    {
        if (isDead || isInvulnerable) return;
        
        currentHealth = Mathf.Max(0, currentHealth - damage);
        OnHealthChanged?.Invoke(currentHealth);
        OnDamageTaken?.Invoke(damage);
        
        Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}/{maxHealth}");
        
        // Activate ragdoll on damage if enabled
        if (enableRagdollOnDamage && ragdollController != null && !ragdollActivated)
        {
            ragdollController.EnableRagdoll();
            ragdollActivated = true;
            OnRagdollActivated?.Invoke();
            
            // Apply impact force if provided
            if (impactPoint != default && impactForce > 0f)
            {
                ragdollController.ApplyImpact(impactPoint, impactDirection, impactForce);
            }
        }
        
        if (currentHealth <= 0)
        {
            Die(impactPoint, impactDirection, impactForce);
        }
    }
    
    public void Heal(float amount)
    {
        if (isDead) return;
        
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth);
        
        Debug.Log($"{gameObject.name} healed {amount}. Health: {currentHealth}/{maxHealth}");
    }
    
    public void SetInvulnerable(bool invulnerable)
    {
        isInvulnerable = invulnerable;
    }
    
    void Die(Vector3 impactPoint = default, Vector3 impactDirection = default, float impactForce = 0f)
    {
        if (isDead) return;
        
        isDead = true;
        
        // Activate ragdoll on death if enabled
        if (enableRagdollOnDeath && ragdollController != null)
        {
            ragdollController.EnableRagdoll();
            
            // Apply death impact force
            if (impactPoint != default)
            {
                ragdollController.ApplyImpact(impactPoint, impactDirection, impactForce + ragdollForceOnDeath);
            }
            else
            {
                // Apply random death force if no impact point
                Vector3 randomDirection = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(0.5f, 1f),
                    Random.Range(-1f, 1f)
                ).normalized;
                ragdollController.ApplyImpact(transform.position, randomDirection, ragdollForceOnDeath);
            }
        }
        
        // Invoke death event
        OnDeath?.Invoke();
        
        Debug.Log($"{gameObject.name} has died!");
        
        // Handle death with delay
        if (deathDelay > 0f)
        {
            Invoke(nameof(HandleDeath), deathDelay);
        }
        else
        {
            HandleDeath();
        }
    }
    
    void HandleDeath()
    {
        // Destroy object if enabled
        if (destroyOnDeath)
        {
            if (destroyDelay > 0f)
            {
                Destroy(gameObject, destroyDelay);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
    
    // Public getters
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public float GetHealthPercentage() => currentHealth / maxHealth;
    public bool IsDead() => isDead;
    public bool IsAlive() => !isDead;
    public bool IsRagdollActive() => ragdollActivated;
}
