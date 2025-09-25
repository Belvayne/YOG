using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Header("Spawn Point Settings")]
    public bool isActive = true;
    public float spawnRadius = 2f;
    public LayerMask groundLayer = 1;
    public bool useGroundDetection = true;
    public float groundCheckHeight = 10f;
    
    [Header("Visual Settings")]
    public bool showGizmos = true;
    public Color gizmoColor = Color.green;
    public float gizmoSize = 1f;
    
    void Start()
    {
        // Ensure spawn point is at ground level if ground detection is enabled
        if (useGroundDetection && groundLayer != 0)
        {
            AdjustToGround();
        }
    }
    
    void AdjustToGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * groundCheckHeight, Vector3.down, out hit, groundCheckHeight * 2f, groundLayer))
        {
            transform.position = hit.point;
        }
    }
    
    public Vector3 GetSpawnPosition()
    {
        if (!isActive) return transform.position;
        
        Vector3 spawnPos = transform.position;
        
        // Add random offset within spawn radius
        if (spawnRadius > 0f)
        {
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            spawnPos += new Vector3(randomCircle.x, 0f, randomCircle.y);
        }
        
        // Adjust to ground if enabled
        if (useGroundDetection && groundLayer != 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(spawnPos + Vector3.up * groundCheckHeight, Vector3.down, out hit, groundCheckHeight * 2f, groundLayer))
            {
                spawnPos.y = hit.point.y;
            }
        }
        
        return spawnPos;
    }
    
    public bool IsActive()
    {
        return isActive;
    }
    
    public void SetActive(bool active)
    {
        isActive = active;
    }
    
    void OnDrawGizmos()
    {
        if (!showGizmos) return;
        
        Gizmos.color = gizmoColor;
        
        // Draw spawn point
        Gizmos.DrawWireSphere(transform.position, gizmoSize);
        
        // Draw spawn radius
        if (spawnRadius > 0f)
        {
            Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.3f);
            Gizmos.DrawSphere(transform.position, spawnRadius);
        }
        
        // Draw ground check line
        if (useGroundDetection)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckHeight);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, gizmoSize);
        
        if (spawnRadius > 0f)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
            Gizmos.DrawSphere(transform.position, spawnRadius);
        }
    }
}
