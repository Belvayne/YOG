using UnityEngine;
using System.Collections;

public class RagdollActivator : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public Rigidbody[] ragdollBodies;

    private bool isRagdoll = false;

    void Awake()
    {
        // Automatically find all rigidbodies in children
        if (ragdollBodies == null || ragdollBodies.Length == 0)
            ragdollBodies = GetComponentsInChildren<Rigidbody>();

        SetRagdoll(false);
    }

    public void SetRagdoll(bool active)
    {
        isRagdoll = active;

        // Disable Animator when activating ragdoll
        if (animator != null)
            animator.enabled = !active;

        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = !active;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    public void ActivateRagdoll(Vector3 hitPosition, Vector3 hitForce)
    {
        //if (isRagdoll) return;

        SetRagdoll(true);

        // Find the closest rigidbody to where it was hit
        Rigidbody closestBody = null;
        float minDist = float.MaxValue;

        foreach (var rb in ragdollBodies)
        {
            float dist = Vector3.Distance(rb.worldCenterOfMass, hitPosition);
            if (dist < minDist)
            {
                minDist = dist;
                closestBody = rb;
            }
        }

        // Apply the force to that part
        if (closestBody != null)
        {
            closestBody.AddForce(hitForce, ForceMode.Impulse);
        }
        // Start coroutine to stop momentum after 5 seconds
        StartCoroutine(StopMomentum());

        // Start sinking and destroy coroutine
        StartCoroutine(SinkAndDestroyCoroutine());
    }

    private IEnumerator SinkAndDestroyCoroutine()
    {
        yield return new WaitForSeconds(10f);
        float sinkDuration = 2f;
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.down * -2f; // Sink 2 units down
        foreach (var rb in ragdollBodies)
        {
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
        while (elapsed < sinkDuration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / sinkDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;
        Destroy(gameObject);
    }

    private IEnumerator StopMomentum()
    {
        yield return new WaitForSeconds(5f);

        foreach (var rb in ragdollBodies)
        {
            rb.interpolation = RigidbodyInterpolation.None;
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        }
    }
}