using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;


public class LevelManager : MonoBehaviour
{
    [Header("UI Document")]
    [SerializeField] private UIDocument uiDocument;

    [Header("Player Spawn Settings")]
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private GameObject PlayerPrefab;

    private GameObject spawnedPlayer;

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
    
    [Header("Pause Settings")]
    [SerializeField] private bool hideCursorDuringGameplay = true;
    
    private ProgressBar hypeMeter;
    private Label killCountText;
    private GroupBox pauseMenu;
    private Button resumeButton;
    private Button settingsButton;
    private Button restartButton;
    private Button quitButton;
    
    private float currentProgress = 0f;
    private float timeSinceLastDecay = 0f;
    private int killCount = 0;
    private bool isPaused = false;
    
    void Start()
    {
        Debug.Log("LevelManager: Loaded character = " + (GameDataManager.Instance.selectedCharacterPrefab != null ? GameDataManager.Instance.selectedCharacterPrefab.name : "None"));

        // Spawn the selected character first
        SpawnSelectedCharacter();

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
        pauseMenu = root.Q<GroupBox>("PauseMenu");
        
        // Get pause menu buttons
        if (pauseMenu != null)
        {
            resumeButton = pauseMenu.Q<Button>("ResumeButton");
            settingsButton = pauseMenu.Q<Button>("SettingsButton");
            restartButton = pauseMenu.Q<Button>("RestartButton");
            quitButton = pauseMenu.Q<Button>("QuitButton");
            
            // Register button callbacks
            if (resumeButton != null)
                resumeButton.clicked += ResumeGame;
            
            if (settingsButton != null)
                settingsButton.clicked += OpenSettings;
            
            if (restartButton != null)
                restartButton.clicked += RestartLevel;
            
            if (quitButton != null)
                quitButton.clicked += QuitGame;
            
            // Hide pause menu initially
            pauseMenu.style.display = DisplayStyle.None;
        }
        else
        {
            Debug.LogError("LevelManager: PauseMenu GroupBox not found in UI Document!");
        }
        
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
        
        // Set initial cursor state
        SetCursorState(!hideCursorDuringGameplay);
    }

    private void SpawnSelectedCharacter()
    {
        // Use fallback if no character is selected
        GameObject Player = PlayerPrefab;

        // Determine spawn position
        Vector3 spawnPosition = playerSpawnPoint != null ? playerSpawnPoint.position : Vector3.zero;
        Quaternion spawnRotation = playerSpawnPoint != null ? playerSpawnPoint.rotation : Quaternion.identity;

        // Spawn the base Player prefab
        spawnedPlayer = Instantiate(Player, spawnPosition, spawnRotation);

        // Find the PlayerCharacter -> PlayerModel hierarchy
        Transform playerCharacter = spawnedPlayer.transform.Find("PlayerCharacter");

        if (playerCharacter == null)
        {
            Debug.LogError("LevelManager: PlayerCharacter child not found in player prefab! Make sure your Player prefab has a child named 'PlayerCharacter'.");
            return;
        }

        Transform playerModel = playerCharacter.Find("PlayerModel");

        if (playerModel == null)
        {
            Debug.LogError("LevelManager: PlayerModel child not found under PlayerCharacter! Make sure PlayerCharacter has a child named 'PlayerModel'.");
            return;
        }

        // Get the selected character prefab from GameDataManager
        GameObject characterPrefab = GameDataManager.Instance.selectedCharacterPrefab;

        if (characterPrefab == null)
        {
            Debug.LogWarning("LevelManager: No character selected in GameDataManager. Player spawned without character model.");
            return;
        }

        // Clear any existing children in PlayerModel
        foreach (Transform child in playerModel)
        {
            Destroy(child.gameObject);
        }

        // Instantiate the selected character model as a child of PlayerModel
        GameObject characterInstance = Instantiate(characterPrefab, playerModel);
        characterInstance.transform.localPosition = Vector3.zero;
        characterInstance.transform.localRotation = Quaternion.identity;
        var controller = GetComponentInChildren<PlayerController>();
        controller.AssignAnimator(characterInstance.GetComponentInChildren<Animator>());

        Debug.Log($"LevelManager: Instantiated character '{characterPrefab.name}' into PlayerModel");

        if (Player == null)
        {
            Debug.LogError("LevelManager: No player prefab available to spawn! Please assign a fallback player prefab.");
            return;
        }
    }

    void Update()
    {
        // Check for pause input (ESC key)
        var keyboard = UnityEngine.InputSystem.Keyboard.current;
        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
        
        // Only update game logic when not paused
        if (!isPaused)
        {
            // Decrease progress by 1 point every second
            timeSinceLastDecay += Time.deltaTime;
            
            if (timeSinceLastDecay >= 1f)
            {
                DecreaseProgress(pointsLossPerSecond);
                timeSinceLastDecay = 0f;
            }
        }
    }
    
    private void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        
        // Show pause menu
        if (pauseMenu != null)
        {
            pauseMenu.style.display = DisplayStyle.Flex;
        }
        
        // Pause BGM
        PauseBGM();
        
        // Show cursor
        SetCursorState(true);
        
        Debug.Log("LevelManager: Game paused.");
    }
    
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        
        // Hide pause menu
        if (pauseMenu != null)
        {
            pauseMenu.style.display = DisplayStyle.None;
        }
        
        // Resume BGM
        ResumeBGM();
        
        // Hide cursor if enabled
        SetCursorState(!hideCursorDuringGameplay);
        
        Debug.Log("LevelManager: Game resumed.");
    }
    
    private void OpenSettings()
    {
        Debug.Log("LevelManager: Opening settings... (Not implemented yet)");
        // TODO: Implement settings menu
    }
    
    private void RestartLevel()
    {
        // Reset time scale before reloading
        Time.timeScale = 1f;
        
        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
        Debug.Log("LevelManager: Restarting level...");
    }
    
    private void QuitGame()
    {
        Debug.Log("LevelManager: Returning to main menu...");
        
        // Reset time scale before loading main menu
        Time.timeScale = 1f;
        
        // Load main menu scene
        SceneManager.LoadScene("MainMenu");
    }
    
    private void SetCursorState(bool visible)
    {
        UnityEngine.Cursor.visible = visible;
        UnityEngine.Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
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
    
    public bool IsPaused()
    {
        return isPaused;
    }
    
    void OnDestroy()
    {
        // Unregister button callbacks to prevent memory leaks
        if (resumeButton != null)
            resumeButton.clicked -= ResumeGame;
        
        if (settingsButton != null)
            settingsButton.clicked -= OpenSettings;
        
        if (restartButton != null)
            restartButton.clicked -= RestartLevel;
        
        if (quitButton != null)
            quitButton.clicked -= QuitGame;
        
        // Reset time scale when destroyed
        Time.timeScale = 1f;
    }
}
