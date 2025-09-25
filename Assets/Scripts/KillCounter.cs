using UnityEngine;
using UnityEngine.Events;

public class KillCounter : MonoBehaviour
{
    [Header("Kill Settings")]
    public int targetKills = 10;
    public int currentKills = 0;
    public bool resetOnStart = true;
    
    [Header("Events")]
    public UnityEvent<int> OnKillAdded;
    public UnityEvent OnTargetReached;
    public UnityEvent OnLevelComplete;
    
    [Header("UI")]
    public bool updateUI = true;
    public string killTextFormat = "Kills: {0}/{1}";
    
    private static KillCounter instance;
    
    public static KillCounter Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<KillCounter>();
            }
            return instance;
        }
    }
    
    void Start()
    {
        if (resetOnStart)
        {
            currentKills = 0;
        }
        
        // Update UI if enabled
        if (updateUI)
        {
            UpdateUI();
        }
        
        Debug.Log($"Kill counter initialized. Target: {targetKills}");
    }
    
    public void AddKill()
    {
        currentKills++;
        
        Debug.Log($"Kill added! Current: {currentKills}/{targetKills}");
        
        // Invoke kill added event
        OnKillAdded?.Invoke(currentKills);
        
        // Update UI
        if (updateUI)
        {
            UpdateUI();
        }
        
        // Check if target reached
        if (currentKills >= targetKills)
        {
            OnTargetReached?.Invoke();
            OnLevelComplete?.Invoke();
            
            Debug.Log("Target kills reached! Level complete!");
        }
    }
    
    public void AddKills(int amount)
    {
        currentKills += amount;
        
        Debug.Log($"Kills added! Current: {currentKills}/{targetKills}");
        
        // Invoke kill added event
        OnKillAdded?.Invoke(currentKills);
        
        // Update UI
        if (updateUI)
        {
            UpdateUI();
        }
        
        // Check if target reached
        if (currentKills >= targetKills)
        {
            OnTargetReached?.Invoke();
            OnLevelComplete?.Invoke();
            
            Debug.Log("Target kills reached! Level complete!");
        }
    }
    
    public void SetKills(int kills)
    {
        currentKills = kills;
        
        Debug.Log($"Kills set to: {currentKills}/{targetKills}");
        
        // Update UI
        if (updateUI)
        {
            UpdateUI();
        }
        
        // Check if target reached
        if (currentKills >= targetKills)
        {
            OnTargetReached?.Invoke();
            OnLevelComplete?.Invoke();
            
            Debug.Log("Target kills reached! Level complete!");
        }
    }
    
    public void SetTargetKills(int target)
    {
        targetKills = target;
        
        Debug.Log($"Target kills set to: {targetKills}");
        
        // Update UI
        if (updateUI)
        {
            UpdateUI();
        }
    }
    
    public void ResetKills()
    {
        currentKills = 0;
        
        Debug.Log("Kills reset to 0");
        
        // Update UI
        if (updateUI)
        {
            UpdateUI();
        }
    }
    
    void UpdateUI()
    {
        // Find UI components and update them
        GameUI gameUI = FindObjectOfType<GameUI>();
        if (gameUI != null)
        {
            gameUI.UpdateKillCounter(currentKills, targetKills);
        }
        
        // Also update any other UI elements that might be listening
        OnKillAdded?.Invoke(currentKills);
    }
    
    // Public getters
    public int GetCurrentKills() => currentKills;
    public int GetTargetKills() => targetKills;
    public int GetRemainingKills() => Mathf.Max(0, targetKills - currentKills);
    public float GetKillProgress() => (float)currentKills / targetKills;
    public bool IsTargetReached() => currentKills >= targetKills;
    
    // Static methods for easy access
    public static void AddKillStatic()
    {
        if (Instance != null)
        {
            Instance.AddKill();
        }
    }
    
    public static void AddKillsStatic(int amount)
    {
        if (Instance != null)
        {
            Instance.AddKills(amount);
        }
    }
    
    public static int GetCurrentKillsStatic()
    {
        return Instance != null ? Instance.GetCurrentKills() : 0;
    }
    
    public static int GetTargetKillsStatic()
    {
        return Instance != null ? Instance.GetTargetKills() : 0;
    }
}
