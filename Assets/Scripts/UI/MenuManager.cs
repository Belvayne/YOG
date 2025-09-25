using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject startMenuPanel;
    public GameObject gameUIPanel;
    public GameObject pauseMenuPanel;
    public GameObject gameOverPanel;
    public GameObject levelCompletePanel;
    
    [Header("Start Menu UI")]
    public Button startGameButton;
    public Button quitGameButton;
    public Button settingsButton;
    public TextMeshProUGUI titleText;
    
    [Header("Game Over UI")]
    public Button restartButton;
    public Button mainMenuButton;
    public Button quitButton;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI finalScoreText;
    
    [Header("Level Complete UI")]
    public Button nextLevelButton;
    public Button mainMenuCompleteButton;
    public TextMeshProUGUI levelCompleteText;
    public TextMeshProUGUI completionTimeText;
    
    [Header("Pause Menu UI")]
    public Button resumeButton;
    public Button mainMenuPauseButton;
    public Button quitPauseButton;
    
    [Header("Settings")]
    public bool hideCursorInGame = true;
    public bool showCursorInMenus = true;
    public KeyCode pauseKey = KeyCode.Escape;
    
    [Header("Input Actions")]
    public InputAction pauseAction;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip buttonClickSound;
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    
    private GameState currentState = GameState.StartMenu;
    private float gameStartTime;
    private int finalKillCount;
    
    public enum GameState
    {
        StartMenu,
        Playing,
        Paused,
        GameOver,
        LevelComplete
    }
    
    void Start()
    {
        InitializeMenus();
        ShowStartMenu();
    }
    
    void Update()
    {
        // Input is now handled by Input System callbacks
    }
    
    void InitializeMenus()
    {
        // Hide all panels initially
        if (startMenuPanel != null) startMenuPanel.SetActive(false);
        if (gameUIPanel != null) gameUIPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        
        // Setup button listeners
        SetupButtonListeners();
        
        // Setup input actions
        SetupInputActions();
        
        // Setup cursor visibility
        UpdateCursorVisibility();
        
        // Play menu music
        if (audioSource != null && menuMusic != null)
        {
            audioSource.clip = menuMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
    
    void SetupButtonListeners()
    {
        // Start Menu buttons
        if (startGameButton != null)
            startGameButton.onClick.AddListener(StartGame);
        
        if (quitGameButton != null)
            quitGameButton.onClick.AddListener(QuitGame);
        
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);
        
        // Game Over buttons
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
        
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ShowStartMenu);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
        
        // Level Complete buttons
        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(NextLevel);
        
        if (mainMenuCompleteButton != null)
            mainMenuCompleteButton.onClick.AddListener(ShowStartMenu);
        
        // Pause Menu buttons
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);
        
        if (mainMenuPauseButton != null)
            mainMenuPauseButton.onClick.AddListener(ShowStartMenu);
        
        if (quitPauseButton != null)
            quitPauseButton.onClick.AddListener(QuitGame);
    }
    
    void SetupInputActions()
    {
        // Create pause action if not assigned
        if (pauseAction == null)
        {
            pauseAction = new InputAction("Pause", InputActionType.Button, "<Keyboard>/escape");
        }
        
        // Enable the action
        pauseAction.Enable();
        
        // Add callback for pause action
        pauseAction.performed += OnPausePerformed;
    }
    
    void OnPausePerformed(InputAction.CallbackContext context)
    {
        if (currentState == GameState.Playing)
        {
            PauseGame();
        }
        else if (currentState == GameState.Paused)
        {
            ResumeGame();
        }
    }
    
    
    public void ShowStartMenu()
    {
        currentState = GameState.StartMenu;
        
        // Show start menu
        if (startMenuPanel != null) startMenuPanel.SetActive(true);
        
        // Hide other panels
        if (gameUIPanel != null) gameUIPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        
        // Pause time
        Time.timeScale = 1f;
        
        // Show cursor
        UpdateCursorVisibility();
        
        // Play menu music
        if (audioSource != null && menuMusic != null)
        {
            audioSource.clip = menuMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
        
        Debug.Log("Showing start menu");
    }
    
    public void StartGame()
    {
        PlayButtonSound();
        
        // Use SceneLoader if available, otherwise start game in current scene
        SceneLoader sceneLoader = FindObjectOfType<SceneLoader>();
        if (sceneLoader != null)
        {
            sceneLoader.LoadGameScene();
        }
        else
        {
            // Fallback: start game in current scene
            StartGameInCurrentScene();
        }
        
        Debug.Log("Starting game...");
    }
    
    void StartGameInCurrentScene()
    {
        currentState = GameState.Playing;
        gameStartTime = Time.time;
        
        // Hide start menu
        if (startMenuPanel != null) startMenuPanel.SetActive(false);
        
        // Show game UI
        if (gameUIPanel != null) gameUIPanel.SetActive(true);
        
        // Resume time
        Time.timeScale = 1f;
        
        // Hide cursor
        UpdateCursorVisibility();
        
        // Play game music
        if (audioSource != null && gameMusic != null)
        {
            audioSource.clip = gameMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
        
        // Start enemy spawning
        EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
        if (spawner != null)
        {
            spawner.StartSpawning();
        }
        
        Debug.Log("Game started in current scene!");
    }
    
    public void PauseGame()
    {
        if (currentState != GameState.Playing) return;
        
        currentState = GameState.Paused;
        
        // Show pause menu
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        
        // Hide game UI
        if (gameUIPanel != null) gameUIPanel.SetActive(false);
        
        // Pause time
        Time.timeScale = 0f;
        
        // Show cursor
        UpdateCursorVisibility();
        
        PlayButtonSound();
        Debug.Log("Game paused");
    }
    
    public void ResumeGame()
    {
        if (currentState != GameState.Paused) return;
        
        currentState = GameState.Playing;
        
        // Hide pause menu
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        
        // Show game UI
        if (gameUIPanel != null) gameUIPanel.SetActive(true);
        
        // Resume time
        Time.timeScale = 1f;
        
        // Hide cursor
        UpdateCursorVisibility();
        
        PlayButtonSound();
        Debug.Log("Game resumed");
    }
    
    public void ShowGameOver()
    {
        currentState = GameState.GameOver;
        
        // Get final score
        KillCounter killCounter = FindObjectOfType<KillCounter>();
        if (killCounter != null)
        {
            finalKillCount = killCounter.GetCurrentKills();
        }
        
        // Show game over panel
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        
        // Hide other panels
        if (gameUIPanel != null) gameUIPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        
        // Update game over text
        if (gameOverText != null)
        {
            gameOverText.text = "Game Over!";
        }
        
        if (finalScoreText != null)
        {
            finalScoreText.text = $"Final Score: {finalKillCount}";
        }
        
        // Pause time
        Time.timeScale = 0f;
        
        // Show cursor
        UpdateCursorVisibility();
        
        Debug.Log("Game over!");
    }
    
    public void ShowLevelComplete()
    {
        currentState = GameState.LevelComplete;
        
        // Get final score
        KillCounter killCounter = FindObjectOfType<KillCounter>();
        if (killCounter != null)
        {
            finalKillCount = killCounter.GetCurrentKills();
        }
        
        // Calculate completion time
        float completionTime = Time.time - gameStartTime;
        
        // Show level complete panel
        if (levelCompletePanel != null) levelCompletePanel.SetActive(true);
        
        // Hide other panels
        if (gameUIPanel != null) gameUIPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        
        // Update level complete text
        if (levelCompleteText != null)
        {
            levelCompleteText.text = "Level Complete!";
        }
        
        if (completionTimeText != null)
        {
            completionTimeText.text = $"Time: {completionTime:F1}s | Kills: {finalKillCount}";
        }
        
        // Pause time
        Time.timeScale = 0f;
        
        // Show cursor
        UpdateCursorVisibility();
        
        Debug.Log("Level complete!");
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        PlayButtonSound();
    }
    
    public void NextLevel()
    {
        Time.timeScale = 1f;
        // You can implement level progression here
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        PlayButtonSound();
    }
    
    public void QuitGame()
    {
        PlayButtonSound();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    public void OpenSettings()
    {
        // You can implement settings menu here
        PlayButtonSound();
        Debug.Log("Settings opened");
    }
    
    void UpdateCursorVisibility()
    {
        bool shouldShowCursor = false;
        
        switch (currentState)
        {
            case GameState.StartMenu:
            case GameState.Paused:
            case GameState.GameOver:
            case GameState.LevelComplete:
                shouldShowCursor = showCursorInMenus;
                break;
            case GameState.Playing:
                shouldShowCursor = !hideCursorInGame;
                break;
        }
        
        Cursor.visible = shouldShowCursor;
        Cursor.lockState = shouldShowCursor ? CursorLockMode.None : CursorLockMode.Locked;
    }
    
    void PlayButtonSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
    
    void OnDestroy()
    {
        // Clean up input actions
        if (pauseAction != null)
        {
            pauseAction.performed -= OnPausePerformed;
            pauseAction.Disable();
            pauseAction.Dispose();
        }
    }
    
    // Public getters
    public GameState GetCurrentState() => currentState;
    public bool IsGamePlaying() => currentState == GameState.Playing;
    public bool IsGamePaused() => currentState == GameState.Paused;
    public int GetFinalScore() => finalKillCount;
}
