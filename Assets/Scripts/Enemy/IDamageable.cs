using UnityEngine;

public interface IDamageable
{
    /// <summary>
    /// Called when this object takes damage.
    /// </summary>
    /// <param name="hitPoint">The world position of the hit.</param>
    /// <param name="hitForce">The direction and strength of the impact.</param>
    /// <param name="damage">Amount of damage dealt.</param>
    void TakeDamage(Vector3 hitPoint, Vector3 hitForce, float damage);

    /// <summary>
    /// Called when this object dies or is destroyed.
    /// </summary>
    void Die();

    /// <summary>
    /// Returns true if the object is dead.
    /// </summary>
    bool IsDead();

    /// <summary>
    /// Gets the current health of the object.
    /// </summary>
    float GetCurrentHealth();

    /// <summary>
    /// Gets the maximum health of the object.
    /// </summary>
    float GetMaxHealth();
}
