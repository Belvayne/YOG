using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class SceneLoader : MonoBehaviour
{
    [Header("Loading Settings")]
    public float minimumLoadTime = 1f;
    public bool showLoadingScreen = true;
    
    [Header("Loading UI")]
    public GameObject loadingPanel;
    public Slider loadingBar;
    public TextMeshProUGUI loadingText;
    public TextMeshProUGUI progressText;
    
    [Header("Loading Messages")]
    public string[] loadingMessages = {
        "Loading...",
        "Preparing game...",
        "Loading assets...",
        "Almost ready...",
        "Starting game..."
    };
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip loadingSound;
    
    private static SceneLoader instance;
    private bool isLoading = false;
    
    public static SceneLoader Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SceneLoader>();
            }
            return instance;
        }
    }
    
    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Hide loading screen initially
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
    }
    
    public void LoadScene(string sceneName)
    {
        if (isLoading) return;
        
        StartCoroutine(LoadSceneAsync(sceneName));
    }
    
    public void LoadGameScene()
    {
        LoadScene("GameScene");
    }
    
    public void LoadMainMenu()
    {
        LoadScene("MainMenu");
    }
    
    public void RestartCurrentScene()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }
    
    IEnumerator LoadSceneAsync(string sceneName)
    {
        isLoading = true;
        
        // Show loading screen
        if (showLoadingScreen && loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }
        
        // Play loading sound
        if (audioSource != null && loadingSound != null)
        {
            audioSource.PlayOneShot(loadingSound);
        }
        
        // Start loading the scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;
        
        float loadProgress = 0f;
        float startTime = Time.time;
        
        // Update loading progress
        while (!asyncLoad.isDone)
        {
            // Calculate progress (0-0.9 for loading, 0.9-1.0 for activation)
            loadProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            
            // Update loading bar
            if (loadingBar != null)
            {
                loadingBar.value = loadProgress;
            }
            
            // Update progress text
            if (progressText != null)
            {
                progressText.text = $"{Mathf.Round(loadProgress * 100)}%";
            }
            
            // Update loading message
            if (loadingText != null && loadingMessages.Length > 0)
            {
                int messageIndex = Mathf.FloorToInt(loadProgress * loadingMessages.Length);
                messageIndex = Mathf.Clamp(messageIndex, 0, loadingMessages.Length - 1);
                loadingText.text = loadingMessages[messageIndex];
            }
            
            // Check if minimum load time has passed and loading is complete
            if (asyncLoad.progress >= 0.9f && Time.time - startTime >= minimumLoadTime)
            {
                asyncLoad.allowSceneActivation = true;
            }
            
            yield return null;
        }
        
        // Wait a frame to ensure scene is fully loaded
        yield return null;
        
        // Hide loading screen
        if (showLoadingScreen && loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
        
        isLoading = false;
    }
    
    public void LoadSceneWithFade(string sceneName, float fadeTime = 1f)
    {
        StartCoroutine(LoadSceneWithFadeCoroutine(sceneName, fadeTime));
    }
    
    IEnumerator LoadSceneWithFadeCoroutine(string sceneName, float fadeTime)
    {
        // Fade out
        yield return StartCoroutine(FadeOut(fadeTime));
        
        // Load scene
        yield return StartCoroutine(LoadSceneAsync(sceneName));
        
        // Fade in
        yield return StartCoroutine(FadeIn(fadeTime));
    }
    
    IEnumerator FadeOut(float duration)
    {
        // This is a simple fade - you can enhance this with proper UI fading
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    
    IEnumerator FadeIn(float duration)
    {
        // This is a simple fade - you can enhance this with proper UI fading
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    
    // Public getters
    public bool IsLoading() => isLoading;
    
    // Static methods for easy access
    public static void LoadSceneStatic(string sceneName)
    {
        if (Instance != null)
        {
            Instance.LoadScene(sceneName);
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }
    
    public static void LoadGameSceneStatic()
    {
        LoadSceneStatic("GameScene");
    }
    
    public static void LoadMainMenuStatic()
    {
        LoadSceneStatic("MainMenu");
    }
}
