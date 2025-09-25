using UnityEngine;

[System.Serializable]
public class BoneSetup
{
    public string boneName;
    public float mass = 1f;
    public float drag = 0.1f;
    public float angularDrag = 0.1f;
    public Vector3 colliderSize = Vector3.one;
    public bool useCapsuleCollider = true;
}

public class EnemyRagdollSetup : MonoBehaviour
{
    [Header("Ragdoll Setup")]
    public BoneSetup[] boneSetups = new BoneSetup[]
    {
        new BoneSetup { boneName = "Hips", mass = 5f, colliderSize = new Vector3(0.3f, 0.2f, 0.3f) },
        new BoneSetup { boneName = "Spine", mass = 3f, colliderSize = new Vector3(0.25f, 0.3f, 0.2f) },
        new BoneSetup { boneName = "Chest", mass = 4f, colliderSize = new Vector3(0.3f, 0.25f, 0.25f) },
        new BoneSetup { boneName = "Head", mass = 1f, colliderSize = new Vector3(0.2f, 0.2f, 0.2f) },
    };
    
    [Header("Auto Setup")]
    public bool setupOnStart = false;
    public bool addRagdollController = true;
    public bool addEnemyHealth = true;
    
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public bool enableRagdollOnDamage = true;
    public bool enableRagdollOnDeath = true;
    public float ragdollForceOnDeath = 15f;
    
    void Start()
    {
        if (setupOnStart)
        {
            SetupRagdoll();
        }
    }
    
    [ContextMenu("Setup Ragdoll")]
    public void SetupRagdoll()
    {
        Debug.Log($"Setting up ragdoll for {gameObject.name}...");
        
        // Setup bones
        SetupBones();
        
        // Add required components
        if (addRagdollController)
        {
            AddRagdollController();
        }
        
        if (addEnemyHealth)
        {
            AddEnemyHealth();
        }
        
        Debug.Log("Ragdoll setup complete!");
    }
    
    void SetupBones()
    {
        foreach (BoneSetup boneSetup in boneSetups)
        {
            Transform bone = FindBone(boneSetup.boneName);
            if (bone != null)
            {
                SetupBone(bone, boneSetup);
            }
            else
            {
                Debug.LogWarning($"Bone '{boneSetup.boneName}' not found in {gameObject.name}");
            }
        }
    }
    
    Transform FindBone(string boneName)
    {
        // Search in all children
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.name.Contains(boneName))
            {
                return child;
            }
        }
        return null;
    }
    
    void SetupBone(Transform bone, BoneSetup setup)
    {
        // Add Rigidbody
        Rigidbody rb = bone.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = bone.gameObject.AddComponent<Rigidbody>();
        }
        
        rb.mass = setup.mass;
        rb.linearDamping = setup.drag;
        rb.angularDamping = setup.angularDrag;
        rb.useGravity = true;
        rb.isKinematic = true; // Will be controlled by RagdollController
        
        // Add Collider
        Collider col = bone.GetComponent<Collider>();
        if (col == null)
        {
            if (setup.useCapsuleCollider)
            {
                col = bone.gameObject.AddComponent<CapsuleCollider>();
                CapsuleCollider capsule = col as CapsuleCollider;
                capsule.radius = setup.colliderSize.x;
                capsule.height = setup.colliderSize.y;
            }
            else
            {
                col = bone.gameObject.AddComponent<BoxCollider>();
                BoxCollider box = col as BoxCollider;
                box.size = setup.colliderSize;
            }
        }
        
        col.isTrigger = false;
        
        Debug.Log($"Setup bone: {bone.name}");
    }
    
    void AddRagdollController()
    {
        RagdollController ragdoll = GetComponent<RagdollController>();
        if (ragdoll == null)
        {
            ragdoll = gameObject.AddComponent<RagdollController>();
        }
        
        // Configure ragdoll settings
        ragdoll.ragdollForce = 10f;
        ragdoll.impactForceMultiplier = 1f;
        ragdoll.explosionRadius = 2f;
        ragdoll.autoRecover = true;
        ragdoll.recoveryTime = 3f;
        
        Debug.Log("Added RagdollController");
    }
    
    void AddEnemyHealth()
    {
        EnemyHealth health = GetComponent<EnemyHealth>();
        if (health == null)
        {
            health = gameObject.AddComponent<EnemyHealth>();
        }
        
        // Configure health settings
        health.maxHealth = maxHealth;
        health.enableRagdollOnDamage = enableRagdollOnDamage;
        health.enableRagdollOnDeath = enableRagdollOnDeath;
        health.ragdollForceOnDeath = ragdollForceOnDeath;
        
        // Assign ragdoll controller
        health.ragdollController = GetComponent<RagdollController>();
        
        Debug.Log("Added EnemyHealth");
    }
    
    [ContextMenu("Test Ragdoll")]
    public void TestRagdoll()
    {
        RagdollController ragdoll = GetComponent<RagdollController>();
        if (ragdoll != null)
        {
            ragdoll.EnableRagdoll();
            Debug.Log("Ragdoll test activated!");
        }
        else
        {
            Debug.LogWarning("No RagdollController found!");
        }
    }
    
    [ContextMenu("Reset Ragdoll")]
    public void ResetRagdoll()
    {
        RagdollController ragdoll = GetComponent<RagdollController>();
        if (ragdoll != null)
        {
            ragdoll.DisableRagdoll();
            Debug.Log("Ragdoll reset!");
        }
        else
        {
            Debug.LogWarning("No RagdollController found!");
        }
    }
}
