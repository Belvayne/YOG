using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KillDebugger : MonoBehaviour
{
    [Header("Debug UI")]
    public TextMeshProUGUI debugText;
    public Button testKillButton;
    public Button resetKillsButton;
    
    [Header("Debug Settings")]
    public bool showDebugInfo = true;
    public float updateInterval = 0.1f;
    
    private KillCounter killCounter;
    private EnemySpawner enemySpawner;
    private float lastUpdateTime;
    
    void Start()
    {
        // Find components
        killCounter = FindObjectOfType<KillCounter>();
        enemySpawner = FindObjectOfType<EnemySpawner>();
        
        // Setup test button
        if (testKillButton != null)
        {
            testKillButton.onClick.AddListener(TestKill);
        }
        
        // Setup reset button
        if (resetKillsButton != null)
        {
            resetKillsButton.onClick.AddListener(ResetKills);
        }
        
        Debug.Log("KillDebugger initialized");
    }
    
    void Update()
    {
        if (showDebugInfo && Time.time - lastUpdateTime >= updateInterval)
        {
            UpdateDebugInfo();
            lastUpdateTime = Time.time;
        }
    }
    
    void UpdateDebugInfo()
    {
        if (debugText == null) return;
        
        string debugInfo = "=== KILL DEBUG INFO ===\n";
        
        // Kill Counter info
        if (killCounter != null)
        {
            debugInfo += $"KillCounter Found: YES\n";
            debugInfo += $"Current Kills: {killCounter.GetCurrentKills()}\n";
            debugInfo += $"Target Kills: {killCounter.GetTargetKills()}\n";
            debugInfo += $"Target Reached: {killCounter.IsTargetReached()}\n";
        }
        else
        {
            debugInfo += $"KillCounter Found: NO\n";
        }
        
        // Enemy Spawner info
        if (enemySpawner != null)
        {
            debugInfo += $"EnemySpawner Found: YES\n";
            debugInfo += $"Active Enemies: {enemySpawner.GetActiveEnemyCount()}\n";
            debugInfo += $"Is Spawning: {enemySpawner.IsSpawning()}\n";
        }
        else
        {
            debugInfo += $"EnemySpawner Found: NO\n";
        }
        
        // GameUI info
        GameUI gameUI = FindObjectOfType<GameUI>();
        if (gameUI != null)
        {
            debugInfo += $"GameUI Found: YES\n";
        }
        else
        {
            debugInfo += $"GameUI Found: NO\n";
        }
        
        // Bullet info
        Bullet[] bullets = FindObjectsOfType<Bullet>();
        debugInfo += $"Active Bullets: {bullets.Length}\n";
        
        // Enemy Health components
        SimpleEnemyHealth[] simpleEnemies = FindObjectsOfType<SimpleEnemyHealth>();
        EnemyHealth[] ragdollEnemies = FindObjectsOfType<EnemyHealth>();
        debugInfo += $"SimpleEnemyHealth Count: {simpleEnemies.Length}\n";
        debugInfo += $"EnemyHealth Count: {ragdollEnemies.Length}\n";
        
        debugText.text = debugInfo;
    }
    
    public void TestKill()
    {
        if (killCounter != null)
        {
            killCounter.AddKill();
            Debug.Log("Test kill added manually");
        }
        else
        {
            Debug.LogWarning("No KillCounter found for test kill");
        }
    }
    
    public void ResetKills()
    {
        if (killCounter != null)
        {
            killCounter.ResetKills();
            Debug.Log("Kills reset manually");
        }
        else
        {
            Debug.LogWarning("No KillCounter found to reset kills");
        }
    }
    
    [ContextMenu("Test Kill Registration")]
    public void TestKillRegistration()
    {
        Debug.Log("=== TESTING KILL REGISTRATION ===");
        
        // Test 1: Check if KillCounter exists
        if (killCounter == null)
        {
            Debug.LogError("TEST FAILED: No KillCounter found in scene!");
            return;
        }
        Debug.Log("✓ KillCounter found");
        
        // Test 2: Check if EnemySpawner exists
        if (enemySpawner == null)
        {
            Debug.LogError("TEST FAILED: No EnemySpawner found in scene!");
            return;
        }
        Debug.Log("✓ EnemySpawner found");
        
        // Test 3: Check if enemies exist
        SimpleEnemyHealth[] simpleEnemies = FindObjectsOfType<SimpleEnemyHealth>();
        EnemyHealth[] ragdollEnemies = FindObjectsOfType<EnemyHealth>();
        
        if (simpleEnemies.Length == 0 && ragdollEnemies.Length == 0)
        {
            Debug.LogWarning("TEST WARNING: No enemies found in scene!");
        }
        else
        {
            Debug.Log($"✓ Found {simpleEnemies.Length} SimpleEnemyHealth and {ragdollEnemies.Length} EnemyHealth components");
        }
        
        // Test 4: Check if bullets exist
        Bullet[] bullets = FindObjectsOfType<Bullet>();
        if (bullets.Length == 0)
        {
            Debug.LogWarning("TEST WARNING: No bullets found in scene!");
        }
        else
        {
            Debug.Log($"✓ Found {bullets.Length} active bullets");
        }
        
        // Test 5: Test manual kill
        int initialKills = killCounter.GetCurrentKills();
        killCounter.AddKill();
        int newKills = killCounter.GetCurrentKills();
        
        if (newKills == initialKills + 1)
        {
            Debug.Log("✓ Manual kill registration works");
        }
        else
        {
            Debug.LogError("TEST FAILED: Manual kill registration failed!");
        }
        
        Debug.Log("=== KILL REGISTRATION TEST COMPLETE ===");
    }
}
