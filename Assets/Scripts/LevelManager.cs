using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("UI Documents")]
    [SerializeField] private UIDocument uiDocument;

    [Header("UI Menu GameObjects")]
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject deathMenuUI;
    [SerializeField] private GameObject settingsMenuUI;

    [Header("Player Spawn Settings")]
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private GameObject PlayerPrefab;

    private GameObject spawnedPlayer;

    [Header("Progress Bar Settings")]
    [SerializeField] private float maxProgress = 100f;
    [SerializeField] private float pointsPerKill = 5f;
    [SerializeField] private float pointsLossPerSecond = 1f;
    
    [Header("Pause Settings")]
    [SerializeField] private bool hideCursorDuringGameplay = true;

    // UI Elements from main gameplay UI
    private ProgressBar hypeMeter;
    private Label killCountText;
    
    // We don't store button references - we bind them dynamically when menus are shown
    
    private float currentProgress = 0f;
    private float timeSinceLastDecay = 0f;
    private int killCount = 0;
    private bool isPaused = false;
    private bool isInSettings = false;

    void Start()
    {
        Debug.Log("LevelManager: Loaded character = " + (GameDataManager.Instance.selectedCharacterPrefab != null ? GameDataManager.Instance.selectedCharacterPrefab.name : "None"));

        // Spawn the selected character first
        SpawnSelectedCharacter();

        // Get main UI Document if not assigned
        if (uiDocument == null)
        {
            uiDocument = GetComponent<UIDocument>();
        }

        if (uiDocument == null)
        {
            Debug.LogError("LevelManager: UIDocument not found! Please assign it in the inspector or add it to this GameObject.");
            return;
        }

        // Get UI elements from the main gameplay UI Document
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

        // Verify menu GameObjects exist but DON'T query buttons yet
        if (pauseMenuUI == null)
        {
            Debug.LogError("LevelManager: Pause Menu UI GameObject not assigned!");
        }
        else
        {
            // Make sure it has a UIDocument component
            if (pauseMenuUI.GetComponent<UIDocument>() == null)
            {
                Debug.LogError("LevelManager: Pause Menu UI GameObject doesn't have a UIDocument component!");
            }
            // Hide pause menu initially
            pauseMenuUI.SetActive(false);
        }

        if (deathMenuUI == null)
        {
            Debug.LogError("LevelManager: Death Menu UI GameObject not assigned!");
        }
        else
        {
            // Make sure it has a UIDocument component
            if (deathMenuUI.GetComponent<UIDocument>() == null)
            {
                Debug.LogError("LevelManager: Death Menu UI GameObject doesn't have a UIDocument component!");
            }
            // Hide death menu initially
            deathMenuUI.SetActive(false);
        }

        if (settingsMenuUI == null)
        {
            Debug.LogError("LevelManager: Settings Menu UI GameObject not assigned!");
        }
        else
        {
            // Make sure it has a UIDocument component
            if (settingsMenuUI.GetComponent<UIDocument>() == null)
            {
                Debug.LogError("LevelManager: Settings Menu UI GameObject doesn't have a UIDocument component!");
            }
            // Hide settings menu initially
            settingsMenuUI.SetActive(false);
        }

        // Update initial UI
        UpdateUI();

        // Set initial cursor state
        SetCursorState(!hideCursorDuringGameplay);
    }

    // Bind pause menu buttons when the menu is shown
    private void BindPauseMenuButtons()
    {
        if (pauseMenuUI == null) return;

        var pauseUIDoc = pauseMenuUI.GetComponent<UIDocument>();
        if (pauseUIDoc == null)
        {
            Debug.LogError("LevelManager: Pause Menu UI has no UIDocument component!");
            return;
        }

        var pauseRoot = pauseUIDoc.rootVisualElement;
        
        var resumeButton = pauseRoot.Q<Button>("ResumeButton");
        var settingsButton = pauseRoot.Q<Button>("SettingsButton");
        var restartButton = pauseRoot.Q<Button>("RestartButton");
        var quitButton = pauseRoot.Q<Button>("QuitButton");

        // Clear any existing callbacks and register new ones
        if (resumeButton != null)
        {
            resumeButton.clicked -= ResumeGame;
            resumeButton.clicked += ResumeGame;
        }
        else
        {
            Debug.LogError("LevelManager: ResumeButton not found in Pause UI!");
        }

        if (settingsButton != null)
        {
            settingsButton.clicked -= OnSettingsClicked;
            settingsButton.clicked += OnSettingsClicked;
        }
        else
        {
            Debug.LogError("LevelManager: SettingsButton not found in Pause UI!");
        }

        if (restartButton != null)
        {
            restartButton.clicked -= RestartLevel;
            restartButton.clicked += RestartLevel;
        }
        else
        {
            Debug.LogError("LevelManager: RestartButton not found in Pause UI!");
        }

        if (quitButton != null)
        {
            quitButton.clicked -= QuitGame;
            quitButton.clicked += QuitGame;
        }
        else
        {
            Debug.LogError("LevelManager: QuitButton not found in Pause UI!");
        }

        Debug.Log("LevelManager: Pause menu buttons bound successfully.");
    }

    // Bind death menu buttons when the menu is shown
    private void BindDeathMenuButtons()
    {
        if (deathMenuUI == null) return;

        var deathUIDoc = deathMenuUI.GetComponent<UIDocument>();
        if (deathUIDoc == null)
        {
            Debug.LogError("LevelManager: Death Menu UI has no UIDocument component!");
            return;
        }

        var deathRoot = deathUIDoc.rootVisualElement;
        
        var restartButton = deathRoot.Q<Button>("RestartButton");
        var quitButton = deathRoot.Q<Button>("QuitButton");

        // Clear any existing callbacks and register new ones
        if (restartButton != null)
        {
            restartButton.clicked -= RestartLevel;
            restartButton.clicked += RestartLevel;
        }
        else
        {
            Debug.LogError("LevelManager: RestartButton not found in Death UI!");
        }

        if (quitButton != null)
        {
            quitButton.clicked -= QuitGame;
            quitButton.clicked += QuitGame;
        }
        else
        {
            Debug.LogError("LevelManager: QuitButton not found in Death UI!");
        }

        Debug.Log("LevelManager: Death menu buttons bound successfully.");
    }

    // Bind settings menu buttons when the menu is shown
    private void BindSettingsMenuButtons()
    {
        if (settingsMenuUI == null) return;

        var settingsUIDoc = settingsMenuUI.GetComponent<UIDocument>();
        if (settingsUIDoc == null)
        {
            Debug.LogError("LevelManager: Settings Menu UI has no UIDocument component!");
            return;
        }

        var settingsRoot = settingsUIDoc.rootVisualElement;
        
        var backButton = settingsRoot.Q<Button>("BackButton");

        // Clear any existing callbacks and register new ones
        if (backButton != null)
        {
            backButton.clicked -= OnSettingsBackClicked;
            backButton.clicked += OnSettingsBackClicked;
            Debug.Log("LevelManager: BackButton bound to OnSettingsBackClicked");
        }
        else
        {
            Debug.LogError("LevelManager: BackButton not found in Settings UI!");
        }

        Debug.Log("LevelManager: Settings menu buttons bound successfully.");
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
        var controller = spawnedPlayer.GetComponentInChildren<PlayerController>();
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
            // If settings menu is open, close it and return to pause menu
            if (isInSettings)
            {
                OnSettingsBackClicked();
            }
            else
            {
                TogglePause();
            }
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
    
    public void TogglePause()
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

    public void DeathMenu()
    {
        Time.timeScale = 0f;

        // Hide main gameplay UI
        if (uiDocument != null && uiDocument.rootVisualElement != null)
        {
            uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        }

        // Show death menu
        if (deathMenuUI != null)
        {
            deathMenuUI.SetActive(true);
            // IMPORTANT: Bind buttons AFTER activating the GameObject
            BindDeathMenuButtons();
            Debug.Log("LevelManager: Death menu shown.");
        }
        else
        {
            Debug.LogError("LevelManager: Cannot show death menu - GameObject is null!");
        }

        // Show cursor
        SetCursorState(true);

        Debug.Log("LevelManager: Player Died.");
    }
    
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        
        // Hide main gameplay UI
        if (uiDocument != null && uiDocument.rootVisualElement != null)
        {
            uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        }
        
        // Show pause menu
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
            // IMPORTANT: Bind buttons AFTER activating the GameObject
            BindPauseMenuButtons();
            Debug.Log("LevelManager: Pause menu shown.");
        }
        else
        {
            Debug.LogError("LevelManager: Cannot show pause menu - GameObject is null!");
        }
        
        // Show cursor
        SetCursorState(true);
        
        Debug.Log("LevelManager: Game paused.");
    }
    
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        
        // Hide pause menu
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
            Debug.Log("LevelManager: Pause menu hidden.");
        }
        
        // Show main gameplay UI
        if (uiDocument != null && uiDocument.rootVisualElement != null)
        {
            uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        }
        
        // Hide cursor if enabled
        SetCursorState(!hideCursorDuringGameplay);
        
        Debug.Log("LevelManager: Game resumed.");
    }

    void OnSettingsClicked()
    {
        Debug.Log("LevelManager: Settings Clicked!");

        isInSettings = true;

        // Hide pause menu
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        // Show settings menu
        if (settingsMenuUI != null)
        {
            settingsMenuUI.SetActive(true);
            // IMPORTANT: Bind settings menu buttons AFTER activating
            BindSettingsMenuButtons();
            Debug.Log("LevelManager: Settings menu shown.");
        }
        else
        {
            Debug.LogError("LevelManager: Cannot show settings menu - GameObject is null!");
        }
    }

    public void OnSettingsBackClicked()
    {
        Debug.Log("LevelManager: Back from Settings!");

        isInSettings = false;

        // Hide settings menu
        if (settingsMenuUI != null)
        {
            settingsMenuUI.SetActive(false);
            Debug.Log("LevelManager: Settings menu hidden.");
        }

        // Show pause menu again
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
            // Re-bind buttons when returning to pause menu
            BindPauseMenuButtons();
            Debug.Log("LevelManager: Pause menu shown.");
        }
    }

    private void RestartLevel()
    {
        Debug.Log("LevelManager: RestartLevel called!");
        
        // Reset time scale before reloading
        Time.timeScale = 1f;
        
        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    private void QuitGame()
    {
        Debug.Log("LevelManager: QuitGame called - Returning to main menu...");
        
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
    
    public bool IsPaused()
    {
        return isPaused;
    }
    
    void OnDestroy()
    {
        // Unregister button callbacks to prevent memory leaks
        if (pauseMenuUI != null)
        {
            var pauseUIDoc = pauseMenuUI.GetComponent<UIDocument>();
            if (pauseUIDoc != null)
            {
                var pauseRoot = pauseUIDoc.rootVisualElement;
                
                var resumeButton = pauseRoot.Q<Button>("ResumeButton");
                var settingsButton = pauseRoot.Q<Button>("SettingsButton");
                var restartButton = pauseRoot.Q<Button>("RestartButton");
                var quitButton = pauseRoot.Q<Button>("QuitButton");

                if (resumeButton != null)
                    resumeButton.clicked -= ResumeGame;
                
                if (settingsButton != null)
                    settingsButton.clicked -= OnSettingsClicked;
                
                if (restartButton != null)
                    restartButton.clicked -= RestartLevel;
                
                if (quitButton != null)
                    quitButton.clicked -= QuitGame;
            }
        }
        
        if (deathMenuUI != null)
        {
            var deathUIDoc = deathMenuUI.GetComponent<UIDocument>();
            if (deathUIDoc != null)
            {
                var deathRoot = deathUIDoc.rootVisualElement;
                
                var restartButton = deathRoot.Q<Button>("RestartButton");
                var quitButton = deathRoot.Q<Button>("QuitButton");

                if (restartButton != null)
                    restartButton.clicked -= RestartLevel;
                
                if (quitButton != null)
                    quitButton.clicked -= QuitGame;
            }
        }
        
        // Reset time scale when destroyed
        Time.timeScale = 1f;
    }
}
