using UnityEngine;
using UnityEngine.Events;

public class SimpleEnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public bool isInvulnerable = false;
    public bool dieOnZeroHealth = true;
    
    [Header("Crazy Physics Integration")]
    public CrazyPhysicsController crazyPhysics;
    public bool enableCrazyPhysicsOnDamage = true;
    public bool enableCrazyPhysicsOnDeath = true;
    public float deathForceMultiplier = 2f;
    
    [Header("Death Settings")]
    public float deathDelay = 0f;
    public bool destroyOnDeath = true;
    public float destroyDelay = 5f;
    
    [Header("Events")]
    public UnityEvent<float> OnHealthChanged;
    public UnityEvent OnDeath;
    public UnityEvent<float> OnDamageTaken;
    public UnityEvent OnCrazyPhysicsActivated;
    
    private float currentHealth;
    private bool isDead = false;
    private bool crazyPhysicsActivated = false;
    
    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);
        
        // Get crazy physics controller if not assigned
        if (crazyPhysics == null)
        {
            crazyPhysics = GetComponent<CrazyPhysicsController>();
        }
    }
    
    public void TakeDamage(float damage, Vector3 impactPoint = default, Vector3 impactDirection = default, float impactForce = 0f)
    {
        if (isDead || isInvulnerable) return;
        
        currentHealth = Mathf.Max(0, currentHealth - damage);
        OnHealthChanged?.Invoke(currentHealth);
        OnDamageTaken?.Invoke(damage);
        
        Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}/{maxHealth}");
        
        // Activate crazy physics on damage if enabled
        if (enableCrazyPhysicsOnDamage && crazyPhysics != null)
        {
            if (!crazyPhysicsActivated)
            {
                crazyPhysicsActivated = true;
                OnCrazyPhysicsActivated?.Invoke();
            }
            
            // Apply crazy physics with impact force
            if (impactPoint != default && impactForce > 0f)
            {
                crazyPhysics.ApplyCrazyPhysics(impactPoint, impactDirection, impactForce);
            }
            else
            {
                // Apply crazy physics at current position
                crazyPhysics.ApplyCrazyPhysics(transform.position, transform.forward, damage * 5f);
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
        
        // Activate crazy physics on death if enabled
        if (enableCrazyPhysicsOnDeath && crazyPhysics != null)
        {
            // Apply death crazy physics with extra force
            if (impactPoint != default)
            {
                crazyPhysics.ApplyCrazyPhysics(impactPoint, impactDirection, impactForce * deathForceMultiplier);
            }
            else
            {
                // Apply random death crazy physics
                Vector3 randomDirection = Random.insideUnitSphere;
                crazyPhysics.ApplyCrazyPhysics(transform.position, randomDirection, maxHealth * deathForceMultiplier);
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
    public bool IsCrazyPhysicsActive() => crazyPhysicsActivated;
}
