using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    public int targetKills = 10;
    public float levelCompleteDelay = 2f;
    public bool autoLoadNextLevel = false;
    public string nextLevelName = "NextLevel";
    
    [Header("Game State")]
    public bool gameStarted = false;
    public bool gamePaused = false;
    public bool levelComplete = false;
    public bool gameOver = false;
    
    [Header("UI References")]
    public GameObject gameUI;
    public GameObject levelCompleteUI;
    public GameObject gameOverUI;
    public GameObject pauseUI;
    
    [Header("Menu Integration")]
    public MenuManager menuManager;
    
    [Header("Level Complete Menu")]
    public GameObject levelCompleteMenuPrefab;
    private GameObject levelCompleteMenuInstance;
    
    [Header("Events")]
    public UnityEvent OnGameStart;
    public UnityEvent OnLevelComplete;
    public UnityEvent OnGameOver;
    public UnityEvent OnGamePause;
    public UnityEvent OnGameResume;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip levelCompleteSound;
    public AudioClip gameOverSound;
    public AudioClip backgroundMusic;
    
    [Header("Input Actions")]
    public InputAction pauseAction;
    public InputAction restartAction;
    
    private KillCounter killCounter;
    private EnemySpawner enemySpawner;
    private PlayerShooting playerShooting;
    private bool isInitialized = false;
    
    void Start()
    {
        InitializeLevel();
    }
    
    void InitializeLevel()
    {
        // Get references
        killCounter = FindObjectOfType<KillCounter>();
        enemySpawner = FindObjectOfType<EnemySpawner>();
        playerShooting = FindObjectOfType<PlayerShooting>();
        menuManager = FindObjectOfType<MenuManager>();
        
        // Set target kills
        if (killCounter != null)
        {
            killCounter.SetTargetKills(targetKills);
            killCounter.OnLevelComplete.AddListener(OnLevelCompleteHandler);
        }
        
        // Start game automatically when level loads
        StartGame();
        
        // Setup input actions
        SetupInputActions();
        
        isInitialized = true;
        Debug.Log("Level initialized!");
    }
    
    void StartGame()
    {
        gameStarted = true;
        gamePaused = false;
        levelComplete = false;
        gameOver = false;
        
        // Hide cursor for gameplay
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        // Show game UI
        if (gameUI != null)
        {
            gameUI.SetActive(true);
        }
        
        // Hide other UIs
        if (levelCompleteUI != null)
        {
            levelCompleteUI.SetActive(false);
        }
        
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
        
        if (pauseUI != null)
        {
            pauseUI.SetActive(false);
        }
        
        // Start enemy spawning
        if (enemySpawner != null)
        {
            enemySpawner.StartSpawning();
        }
        
        // Play background music
        if (audioSource != null && backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
        
        OnGameStart?.Invoke();
        Debug.Log("Game started!");
    }
    
    void OnLevelCompleteHandler()
    {
        if (levelComplete) return;
        
        levelComplete = true;
        
        // Stop enemy spawning
        if (enemySpawner != null)
        {
            enemySpawner.StopSpawning();
        }
        
        // Use MenuManager to show level complete
        if (menuManager != null)
        {
            menuManager.ShowLevelComplete();
        }
        else
        {
            // Fallback: spawn a simple level-complete menu with Restart/Main Menu/Quit
            ShowLevelCompleteMenuFallback();
        }
        
        // Play level complete sound
        if (audioSource != null && levelCompleteSound != null)
        {
            audioSource.PlayOneShot(levelCompleteSound);
        }
        
        OnLevelComplete?.Invoke();
        Debug.Log("Level complete!");
        
        // Auto load next level or wait for input
        if (autoLoadNextLevel)
        {
            Invoke(nameof(LoadNextLevel), levelCompleteDelay);
        }
    }

    void ShowLevelCompleteMenuFallback()
    {
        // Pause gameplay and show cursor
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        // Hide in-game UI if referenced
        if (gameUI != null) gameUI.SetActive(false);
        
        // If a prefab is provided, instantiate it; otherwise build a minimal UI
        if (levelCompleteMenuPrefab != null)
        {
            levelCompleteMenuInstance = Instantiate(levelCompleteMenuPrefab);
            var menu = levelCompleteMenuInstance.GetComponent<LevelCompleteMenu>();
            if (menu != null)
            {
                menu.Bind(this);
            }
        }
        else
        {
            // Create a minimal canvas with three buttons
            var canvasGO = new GameObject("LevelCompleteMenu_Fallback");
            levelCompleteMenuInstance = canvasGO;
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            
            // Panel background
            var panel = new GameObject("Panel");
            panel.transform.SetParent(canvasGO.transform, false);
            var panelRT = panel.AddComponent<RectTransform>();
            panelRT.anchorMin = new Vector2(0.5f, 0.5f);
            panelRT.anchorMax = new Vector2(0.5f, 0.5f);
            panelRT.sizeDelta = new Vector2(400, 260);
            panelRT.anchoredPosition = Vector2.zero;
            var panelImg = panel.AddComponent<UnityEngine.UI.Image>();
            panelImg.color = new Color(0f, 0f, 0f, 0.8f);
            
            // Helper local function to create a button
            UnityEngine.UI.Button CreateButton(string name, string label, Vector2 pos)
            {
                var btnGO = new GameObject(name);
                btnGO.transform.SetParent(panel.transform, false);
                var rt = btnGO.AddComponent<RectTransform>();
                rt.sizeDelta = new Vector2(280, 60);
                rt.anchoredPosition = pos;
                var img = btnGO.AddComponent<UnityEngine.UI.Image>();
                img.color = new Color(1f, 1f, 1f, 0.15f);
                var btn = btnGO.AddComponent<UnityEngine.UI.Button>();
                
                var textGO = new GameObject("Text");
                textGO.transform.SetParent(btnGO.transform, false);
                var trt = textGO.AddComponent<RectTransform>();
                trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one;
                trt.offsetMin = Vector2.zero; trt.offsetMax = Vector2.zero;
                var tmp = textGO.AddComponent<TMPro.TextMeshProUGUI>();
                tmp.text = label;
                tmp.alignment = TMPro.TextAlignmentOptions.Center;
                tmp.fontSize = 32f;
                tmp.color = Color.white;
                return btn;
            }
            
            var restartBtn = CreateButton("RestartButton", "Restart", new Vector2(0, 70));
            restartBtn.onClick.AddListener(() => { Time.timeScale = 1f; RestartLevel(); });
            var mainMenuBtn = CreateButton("MainMenuButton", "Main Menu", new Vector2(0, 0));
            mainMenuBtn.onClick.AddListener(() => { Time.timeScale = 1f; LoadMainMenu(); });
            var quitBtn = CreateButton("QuitButton", "Quit", new Vector2(0, -70));
            quitBtn.onClick.AddListener(() => {
                Time.timeScale = 1f;
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            });
        }
    }
    
    public void LoadNextLevel()
    {
        if (!string.IsNullOrEmpty(nextLevelName))
        {
            SceneManager.LoadScene(nextLevelName);
        }
        else
        {
            Debug.LogWarning("Next level name not set!");
        }
    }
    
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    public void PauseGame()
    {
        if (gamePaused || levelComplete || gameOver) return;
        
        gamePaused = true;
        Time.timeScale = 0f;
        
        // Show cursor for pause menu
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        // Show pause UI
        if (pauseUI != null)
        {
            pauseUI.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No pause UI assigned to LevelManager! Please assign a pause UI in the inspector.");
        }
        
        // Hide game UI
        if (gameUI != null)
        {
            gameUI.SetActive(false);
        }
        
        OnGamePause?.Invoke();
        Debug.Log("Game paused!");
    }
    
    public void ResumeGame()
    {
        if (!gamePaused) return;
        
        gamePaused = false;
        Time.timeScale = 1f;
        
        // Hide cursor for gameplay
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        // Hide pause UI
        if (pauseUI != null)
        {
            pauseUI.SetActive(false);
        }
        
        // Show game UI
        if (gameUI != null)
        {
            gameUI.SetActive(true);
        }
        
        OnGameResume?.Invoke();
        Debug.Log("Game resumed!");
    }
    
    // Helper method to wire up pause menu buttons
    public void SetupPauseMenuButtons(UnityEngine.UI.Button resumeButton, UnityEngine.UI.Button restartButton, UnityEngine.UI.Button mainMenuButton)
    {
        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(ResumeGame);
        }
        
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartLevel);
        }
        
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(LoadMainMenu);
        }        
    }
    
    public void TogglePause()
    {
        if (gamePaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    
    public void GameOver()
    {
        if (gameOver || levelComplete) return;
        
        gameOver = true;
        
        // Stop enemy spawning
        if (enemySpawner != null)
        {
            enemySpawner.StopSpawning();
        }
        
        // Use MenuManager to show game over
        if (menuManager != null)
        {
            menuManager.ShowGameOver();
        }
        else
        {
            // Fallback to old method
            if (gameOverUI != null)
            {
                gameOverUI.SetActive(true);
            }
            
            if (gameUI != null)
            {
                gameUI.SetActive(false);
            }
        }
        
        // Play game over sound
        if (audioSource != null && gameOverSound != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }
        
        OnGameOver?.Invoke();
        Debug.Log("Game over!");
    }
    
    void SetupInputActions()
    {
        // Create pause action if not assigned
        if (pauseAction == null)
        {
            pauseAction = new InputAction("Pause", InputActionType.Button, "<Keyboard>/escape");
        }
        
        // Create restart action if not assigned
        if (restartAction == null)
        {
            restartAction = new InputAction("Restart", InputActionType.Button, "<Keyboard>/r");
        }
        
        // Enable the actions
        pauseAction.Enable();
        restartAction.Enable();
        
        // Add callbacks
        pauseAction.performed += OnPausePerformed;
        restartAction.performed += OnRestartPerformed;
        
        Debug.Log("LevelManager input actions setup complete");
    }
    
    void OnPausePerformed(InputAction.CallbackContext context)
    {
        TogglePause();
    }
    
    void OnRestartPerformed(InputAction.CallbackContext context)
    {
        RestartLevel();
    }
    
    void Update()
    {
        // Input is now handled by Input System callbacks
        // No need for manual input checking here
    }
    
    // Public getters
    public bool IsGameStarted() => gameStarted;
    public bool IsGamePaused() => gamePaused;
    public bool IsLevelComplete() => levelComplete;
    public bool IsGameOver() => gameOver;
    public int GetTargetKills() => targetKills;
    public int GetCurrentKills() => killCounter != null ? killCounter.GetCurrentKills() : 0;
    public float GetKillProgress() => killCounter != null ? killCounter.GetKillProgress() : 0f;
    
    void OnDestroy()
    {
        // Clean up input actions
        if (pauseAction != null)
        {
            pauseAction.performed -= OnPausePerformed;
            pauseAction.Disable();
            pauseAction.Dispose();
        }
        
        if (restartAction != null)
        {
            restartAction.performed -= OnRestartPerformed;
            restartAction.Disable();
            restartAction.Dispose();
        }
        
        // Reset time scale when object is destroyed
        Time.timeScale = 1f;
    }
}
