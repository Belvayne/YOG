using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KillTestScript : MonoBehaviour
{
    [Header("Test UI")]
    public TextMeshProUGUI testLogText;
    public Button testKillButton;
    public Button testEnemyDeathButton;
    public Button clearLogButton;
    
    private string logText = "";
    private int logLineCount = 0;
    private const int maxLogLines = 20;
    
    void Start()
    {
        // Setup test buttons
        if (testKillButton != null)
        {
            testKillButton.onClick.AddListener(TestDirectKill);
        }
        
        if (testEnemyDeathButton != null)
        {
            testEnemyDeathButton.onClick.AddListener(TestEnemyDeath);
        }
        
        if (clearLogButton != null)
        {
            clearLogButton.onClick.AddListener(ClearLog);
        }
        
        AddLog("KillTestScript initialized");
        AddLog("Click buttons to test kill system");
    }
    
    void Update()
    {
        // Update log display
        if (testLogText != null)
        {
            testLogText.text = logText;
        }
    }
    
    void AddLog(string message)
    {
        logLineCount++;
        if (logLineCount > maxLogLines)
        {
            // Remove oldest lines
            int firstNewline = logText.IndexOf('\n');
            if (firstNewline >= 0)
            {
                logText = logText.Substring(firstNewline + 1);
            }
            logLineCount--;
        }
        
        logText += $"{logLineCount}: {message}\n";
        Debug.Log($"KillTest: {message}");
    }
    
    public void TestDirectKill()
    {
        AddLog("=== Testing Direct Kill ===");
        
        KillCounter killCounter = FindObjectOfType<KillCounter>();
        if (killCounter != null)
        {
            int beforeKills = killCounter.GetCurrentKills();
            AddLog($"KillCounter found. Kills before: {beforeKills}");
            
            killCounter.AddKill();
            
            int afterKills = killCounter.GetCurrentKills();
            AddLog($"Kill added. Kills after: {afterKills}");
            
            if (afterKills == beforeKills + 1)
            {
                AddLog("✓ Direct kill test PASSED");
            }
            else
            {
                AddLog("✗ Direct kill test FAILED");
            }
        }
        else
        {
            AddLog("✗ KillCounter not found!");
        }
        
        AddLog("=== Direct Kill Test Complete ===");
    }
    
    public void TestEnemyDeath()
    {
        AddLog("=== Testing Enemy Death ===");
        
        // Find an enemy
        SimpleEnemyHealth simpleEnemy = FindObjectOfType<SimpleEnemyHealth>();
        EnemyHealth enemyHealth = FindObjectOfType<EnemyHealth>();
        
        if (simpleEnemy != null)
        {
            AddLog($"Found SimpleEnemyHealth: {simpleEnemy.name}");
            AddLog($"Current health: {simpleEnemy.GetCurrentHealth()}");
            AddLog("Simulating enemy death...");
            
            // Simulate death by taking enough damage
            simpleEnemy.TakeDamage(simpleEnemy.GetMaxHealth() + 100f);
        }
        else if (enemyHealth != null)
        {
            AddLog($"Found EnemyHealth: {enemyHealth.name}");
            AddLog($"Current health: {enemyHealth.GetCurrentHealth()}");
            AddLog("Simulating enemy death...");
            
            // Simulate death by taking enough damage
            enemyHealth.TakeDamage(enemyHealth.GetMaxHealth() + 100f);
        }
        else
        {
            AddLog("✗ No enemies found in scene!");
        }
        
        AddLog("=== Enemy Death Test Complete ===");
    }
    
    public void ClearLog()
    {
        logText = "";
        logLineCount = 0;
        AddLog("Log cleared");
    }
    
    [ContextMenu("Run Full Kill Test")]
    public void RunFullKillTest()
    {
        AddLog("=== RUNNING FULL KILL TEST ===");
        
        // Test 1: Check components
        AddLog("1. Checking components...");
        
        KillCounter killCounter = FindObjectOfType<KillCounter>();
        EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
        GameUI gameUI = FindObjectOfType<GameUI>();
        
        AddLog($"KillCounter: {(killCounter != null ? "Found" : "Missing")}");
        AddLog($"EnemySpawner: {(spawner != null ? "Found" : "Missing")}");
        AddLog($"GameUI: {(gameUI != null ? "Found" : "Missing")}");
        
        // Test 2: Check enemies
        AddLog("2. Checking enemies...");
        SimpleEnemyHealth[] simpleEnemies = FindObjectsOfType<SimpleEnemyHealth>();
        EnemyHealth[] ragdollEnemies = FindObjectsOfType<EnemyHealth>();
        
        AddLog($"SimpleEnemyHealth count: {simpleEnemies.Length}");
        AddLog($"EnemyHealth count: {ragdollEnemies.Length}");
        
        // Test 3: Check UI elements
        if (gameUI != null)
        {
            AddLog("3. Checking GameUI elements...");
            AddLog($"killCountText: {(gameUI.killCountText != null ? "Assigned" : "Null")}");
            AddLog($"killProgressBar: {(gameUI.killProgressBar != null ? "Assigned" : "Null")}");
            AddLog($"levelCompleteText: {(gameUI.levelCompleteText != null ? "Assigned" : "Null")}");
        }
        
        // Test 4: Test direct kill
        AddLog("4. Testing direct kill...");
        TestDirectKill();
        
        AddLog("=== FULL KILL TEST COMPLETE ===");
    }
}
