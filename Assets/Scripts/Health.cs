using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public bool isInvulnerable = false;
    
    [Header("Events")]
    public UnityEvent<float> OnHealthChanged;
    public UnityEvent OnDeath;
    public UnityEvent<float> OnDamageTaken;
    
    private float currentHealth;
    private bool isDead = false;
    
    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);
    }
    
    public void TakeDamage(float damage)
    {
        if (isDead || isInvulnerable) return;
        
        currentHealth = Mathf.Max(0, currentHealth - damage);
        OnHealthChanged?.Invoke(currentHealth);
        OnDamageTaken?.Invoke(damage);
        
        Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}/{maxHealth}");
        
        if (currentHealth <= 0)
        {
            Die();
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
    
    void Die()
    {
        if (isDead) return;
        
        isDead = true;
        OnDeath?.Invoke();
        
        Debug.Log($"{gameObject.name} has died!");
        
        // You can add death effects here
        // For example: play death animation, spawn death particles, etc.
    }
    
    // Public getters
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public float GetHealthPercentage() => currentHealth / maxHealth;
    public bool IsDead() => isDead;
    public bool IsAlive() => !isDead;
}
