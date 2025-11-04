using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement; // only if you plan to load a scene

public class MainMenuUI : MonoBehaviour
{
    public AudioSource clickSource;   // assign in Inspector
    public AudioClip clickClip;       // assign in Inspector

    private Button characterButton;
    private Button startButton;
    private Button quitButton;

    [SerializeField] private GameObject CameraAnimation;

    void Start()
    {
        // Access root of UIDocument
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Get buttons by their names from your UXML
        characterButton = root.Q<Button>("CharacterSelectButton");
        startButton = root.Q<Button>("StartButton");
        quitButton = root.Q<Button>("QuitButton");

        // Add click event listeners
        characterButton.clicked += OnCharacterClicked;
        startButton.clicked += OnStartClicked;
        quitButton.clicked += OnQuitClicked;
    }

    void PlayClick()
    {
        clickSource.PlayOneShot(clickClip);
    }

    void OnCharacterClicked()
    {
        PlayClick();
        Debug.Log("Character Select Clicked!");
        if (CameraAnimation != null)
        {
            CameraAnimation.SetActive(true);
        }
    }

    void OnStartClicked()
    {
        PlayClick();
        Debug.Log("Start Game Clicked!");
        // Example: SceneManager.LoadScene("GameScene");
    }

    void OnQuitClicked()
    {
        PlayClick();
        Debug.Log("Quit Game Clicked!");
        Application.Quit();
    }
}
