using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Button startGameButton;
    public Button quitGameButton;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI versionText;
    
    [Header("Background")]
    public GameObject backgroundImage;
    public Color backgroundColor = Color.black;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip buttonClickSound;
    public AudioClip backgroundMusic;
    
    [Header("Settings")]
    public string gameVersion = "1.0.0";
    public string gameTitle = "YOG Game";
    public string gameSceneName = "GameScene";
    
    void Start()
    {
        InitializeMainMenu();
    }
    
    void InitializeMainMenu()
    {
        // Setup UI elements
        SetupUI();
        
        // Setup button listeners
        SetupButtonListeners();
        
        // Setup background
        SetupBackground();
        
        // Setup audio
        SetupAudio();
        
        // Show cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        Debug.Log("Main menu initialized");
    }
    
    void SetupUI()
    {
        // Set title text
        if (titleText != null)
        {
            titleText.text = gameTitle;
        }
        
        // Set version text
        if (versionText != null)
        {
            versionText.text = $"Version {gameVersion}";
        }
    }
    
    void SetupButtonListeners()
    {
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(StartGame);
        }
        
        if (quitGameButton != null)
        {
            quitGameButton.onClick.AddListener(QuitGame);
        }
    }
    
    void SetupBackground()
    {
        // Set camera background color
        Camera.main.backgroundColor = backgroundColor;
        
        // Setup background image if present
        if (backgroundImage != null)
        {
            backgroundImage.SetActive(true);
        }
    }
    
    void SetupAudio()
    {
        // Get or create audio source
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        // Play background music
        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
    
    public void StartGame()
    {
        PlayButtonSound();
        
        // Use SceneLoader if available, otherwise load directly
        SceneLoader sceneLoader = FindObjectOfType<SceneLoader>();
        if (sceneLoader != null)
        {
            sceneLoader.LoadGameScene();
        }
        else
        {
            SceneManager.LoadScene(gameSceneName);
        }
        
        Debug.Log("Starting game...");
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
    
    
    void PlayButtonSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
    
    // Public getters
    public string GetGameTitle() => gameTitle;
    public string GetGameVersion() => gameVersion;
}
