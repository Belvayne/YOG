using UnityEngine;
using System.Collections;

public class SSRBLogic : EnemyController
{
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
        Destroy(gameObject);
        // Call your explosion logic here
        // Destroy(gameObject);
    }
}