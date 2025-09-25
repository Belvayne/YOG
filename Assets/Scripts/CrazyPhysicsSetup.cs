using UnityEngine;

public class CrazyPhysicsSetup : MonoBehaviour
{
    [Header("Auto Setup")]
    public bool setupOnStart = false;
    public bool addCrazyPhysicsController = true;
    public bool addSimpleEnemyHealth = true;
    
    [Header("Crazy Physics Settings")]
    public float baseForceMultiplier = 1f;
    public float explosionForce = 15f;
    public float explosionRadius = 3f;
    public float upwardForce = 5f;
    public float spinForce = 10f;
    public float bounceForce = 8f;
    
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public bool enableCrazyPhysicsOnDamage = true;
    public bool enableCrazyPhysicsOnDeath = true;
    public float deathForceMultiplier = 2f;
    
    [Header("Physics Material")]
    public PhysicsMaterial bouncyMaterial;
    
    void Start()
    {
        if (setupOnStart)
        {
            SetupCrazyPhysics();
        }
    }
    
    [ContextMenu("Setup Crazy Physics")]
    public void SetupCrazyPhysics()
    {
        Debug.Log($"Setting up crazy physics for {gameObject.name}...");
        
        // Add required components
        if (addCrazyPhysicsController)
        {
            AddCrazyPhysicsController();
        }
        
        if (addSimpleEnemyHealth)
        {
            AddSimpleEnemyHealth();
        }
        
        // Setup physics material
        SetupPhysicsMaterial();
        
        Debug.Log("Crazy physics setup complete!");
    }
    
    void AddCrazyPhysicsController()
    {
        CrazyPhysicsController crazyPhysics = GetComponent<CrazyPhysicsController>();
        if (crazyPhysics == null)
        {
            crazyPhysics = gameObject.AddComponent<CrazyPhysicsController>();
        }
        
        // Configure crazy physics settings
        crazyPhysics.baseForceMultiplier = baseForceMultiplier;
        crazyPhysics.explosionForce = explosionForce;
        crazyPhysics.explosionRadius = explosionRadius;
        crazyPhysics.upwardForce = upwardForce;
        crazyPhysics.spinForce = spinForce;
        crazyPhysics.bounceForce = bounceForce;
        
        Debug.Log("Added CrazyPhysicsController");
    }
    
    void AddSimpleEnemyHealth()
    {
        SimpleEnemyHealth health = GetComponent<SimpleEnemyHealth>();
        if (health == null)
        {
            health = gameObject.AddComponent<SimpleEnemyHealth>();
        }
        
        // Configure health settings
        health.maxHealth = maxHealth;
        health.enableCrazyPhysicsOnDamage = enableCrazyPhysicsOnDamage;
        health.enableCrazyPhysicsOnDeath = enableCrazyPhysicsOnDeath;
        health.deathForceMultiplier = deathForceMultiplier;
        
        // Assign crazy physics controller
        health.crazyPhysics = GetComponent<CrazyPhysicsController>();
        
        Debug.Log("Added SimpleEnemyHealth");
    }
    
    void SetupPhysicsMaterial()
    {
        Collider col = GetComponent<Collider>();
        if (col != null && bouncyMaterial != null)
        {
            col.material = bouncyMaterial;
            Debug.Log("Applied bouncy physics material");
        }
    }
    
    [ContextMenu("Test Crazy Physics")]
    public void TestCrazyPhysics()
    {
        CrazyPhysicsController crazyPhysics = GetComponent<CrazyPhysicsController>();
        if (crazyPhysics != null)
        {
            Vector3 testPoint = transform.position + Random.insideUnitSphere * 2f;
            Vector3 testDirection = Random.insideUnitSphere;
            float testForce = Random.Range(10f, 20f);
            
            crazyPhysics.ApplyCrazyPhysics(testPoint, testDirection, testForce);
            Debug.Log("Crazy physics test activated!");
        }
        else
        {
            Debug.LogWarning("No CrazyPhysicsController found!");
        }
    }
    
    [ContextMenu("Create Bouncy Material")]
    public void CreateBouncyMaterial()
    {
        PhysicsMaterial bouncy = new PhysicsMaterial("BouncyMaterial");
        bouncy.bounciness = 0.8f;
        bouncy.bounceCombine = PhysicsMaterialCombine.Maximum;
        bouncy.dynamicFriction = 0.1f;
        bouncy.frictionCombine = PhysicsMaterialCombine.Minimum;
        
        // Save as asset
        #if UNITY_EDITOR
        UnityEditor.AssetDatabase.CreateAsset(bouncy, "Assets/BouncyMaterial.asset");
        UnityEditor.AssetDatabase.SaveAssets();
        #endif
        
        bouncyMaterial = bouncy;
        Debug.Log("Created bouncy physics material!");
    }
}
