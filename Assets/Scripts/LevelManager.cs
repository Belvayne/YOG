using UnityEngine;
using UnityEngine.UIElements;

public class LevelManager : MonoBehaviour
{
    [Header("UI Document")]
    [SerializeField] private UIDocument uiDocument;
    
    [Header("Progress Bar Settings")]
    [SerializeField] private float maxProgress = 100f;
    [SerializeField] private float pointsPerKill = 5f;
    [SerializeField] private float pointsLossPerSecond = 1f;
    
    [Header("Audio Settings")]
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioClip bgmClip;
    [SerializeField] private bool playBGMOnStart = true;
    [SerializeField] private bool loopBGM = true;
    [SerializeField] [Range(0f, 1f)] private float bgmVolume = 0.5f;
    
    private ProgressBar hypeMeter;
    private Label killCountText;
    private float currentProgress = 0f;
    private float timeSinceLastDecay = 0f;
    private int killCount = 0;
    
    void Start()
    {
        // Get UI Document if not assigned
        if (uiDocument == null)
        {
            uiDocument = GetComponent<UIDocument>();
        }
        
        if (uiDocument == null)
        {
            Debug.LogError("LevelManager: UIDocument not found! Please assign it in the inspector or add it to this GameObject.");
            return;
        }
        
        // Get UI elements from the UI Document
        var root = uiDocument.rootVisualElement;
        hypeMeter = root.Q<ProgressBar>();
        killCountText = root.Q<Label>("KillCountText");
        
        if (hypeMeter == null)
        {
            Debug.LogError("LevelManager: ProgressBar not found in UI Document!");
        }
        else
        {
            // Initialize progress bar
            hypeMeter.lowValue = 0f;
            hypeMeter.highValue = maxProgress;
            hypeMeter.value = currentProgress;
        }
        
        if (killCountText == null)
        {
            Debug.LogError("LevelManager: KillCountText label not found in UI Document!");
        }
        
        // Initialize BGM AudioSource
        InitializeBGM();
        
        // Update initial UI
        UpdateUI();
        
        // Play BGM if enabled
        if (playBGMOnStart)
        {
            PlayBGM();
        }
    }
    
    void Update()
    {
        // Decrease progress by 1 point every second
        timeSinceLastDecay += Time.deltaTime;
        
        if (timeSinceLastDecay >= 1f)
        {
            DecreaseProgress(pointsLossPerSecond);
            timeSinceLastDecay = 0f;
        }
    }
    
    private void InitializeBGM()
    {
        // Create AudioSource if not assigned
        if (bgmAudioSource == null)
        {
            bgmAudioSource = gameObject.GetComponent<AudioSource>();
            
            if (bgmAudioSource == null)
            {
                bgmAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        // Configure AudioSource for BGM
        if (bgmAudioSource != null)
        {
            bgmAudioSource.playOnAwake = false;
            bgmAudioSource.loop = loopBGM;
            bgmAudioSource.volume = bgmVolume;
            
            if (bgmClip != null)
            {
                bgmAudioSource.clip = bgmClip;
            }
        }
    }
    
    public void PlayBGM()
    {
        if (bgmAudioSource != null && bgmClip != null)
        {
            if (!bgmAudioSource.isPlaying)
            {
                bgmAudioSource.Play();
                Debug.Log("LevelManager: BGM started playing.");
            }
        }
        else
        {
            Debug.LogWarning("LevelManager: Cannot play BGM - AudioSource or AudioClip is missing!");
        }
    }
    
    public void StopBGM()
    {
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Stop();
            Debug.Log("LevelManager: BGM stopped.");
        }
    }
    
    public void PauseBGM()
    {
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Pause();
            Debug.Log("LevelManager: BGM paused.");
        }
    }
    
    public void ResumeBGM()
    {
        if (bgmAudioSource != null && !bgmAudioSource.isPlaying)
        {
            bgmAudioSource.UnPause();
            Debug.Log("LevelManager: BGM resumed.");
        }
    }
    
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        
        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume = bgmVolume;
        }
    }
    
    public void SetBGMClip(AudioClip clip)
    {
        bgmClip = clip;
        
        if (bgmAudioSource != null)
        {
            bool wasPlaying = bgmAudioSource.isPlaying;
            bgmAudioSource.Stop();
            bgmAudioSource.clip = bgmClip;
            
            if (wasPlaying)
            {
                bgmAudioSource.Play();
            }
        }
    }
    
    public void OnEnemyKilled()
    {
        // Increment kill counter
        killCount++;
        
        // Increase progress by 5 points for each kill
        IncreaseProgress(pointsPerKill);
        
        // Update kill count display
        if (killCountText != null)
        {
            killCountText.text = killCount.ToString();
        }
        
        Debug.Log($"Enemy killed! Kill count: {killCount}. Progress increased by {pointsPerKill}. Current progress: {currentProgress}/{maxProgress}");
    }
    
    private void IncreaseProgress(float amount)
    {
        currentProgress = Mathf.Clamp(currentProgress + amount, 0f, maxProgress);
        UpdateProgressBar();
    }
    
    private void DecreaseProgress(float amount)
    {
        currentProgress = Mathf.Clamp(currentProgress - amount, 0f, maxProgress);
        UpdateProgressBar();
    }
    
    private void UpdateProgressBar()
    {
        if (hypeMeter != null)
        {
            hypeMeter.value = currentProgress;
        }
    }
    
    private void UpdateUI()
    {
        UpdateProgressBar();
        
        if (killCountText != null)
        {
            killCountText.text = killCount.ToString();
        }
    }
    
    // Public methods for external control
    public void SetProgress(float value)
    {
        currentProgress = Mathf.Clamp(value, 0f, maxProgress);
        UpdateProgressBar();
    }
    
    public float GetProgress()
    {
        return currentProgress;
    }
    
    public float GetProgressPercentage()
    {
        return currentProgress / maxProgress;
    }
    
    public void ResetProgress()
    {
        currentProgress = 0f;
        UpdateProgressBar();
    }
    
    public int GetKillCount()
    {
        return killCount;
    }
    
    public bool IsBGMPlaying()
    {
        return bgmAudioSource != null && bgmAudioSource.isPlaying;
    }
}
