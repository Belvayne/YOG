using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

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
        
        isInitialized = true;
        Debug.Log("Level initialized!");
    }
    
    void StartGame()
    {
        gameStarted = true;
        gamePaused = false;
        levelComplete = false;
        gameOver = false;
        
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
            // Fallback to old method
            if (levelCompleteUI != null)
            {
                levelCompleteUI.SetActive(true);
            }
            
            if (gameUI != null)
            {
                gameUI.SetActive(false);
            }
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
        
        // Show pause UI
        if (pauseUI != null)
        {
            pauseUI.SetActive(true);
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
    
    void Update()
    {
        // Handle pause input
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        
        // Handle restart input
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }
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
        // Reset time scale when object is destroyed
        Time.timeScale = 1f;
    }
}
