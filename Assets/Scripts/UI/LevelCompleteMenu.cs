using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelCompleteMenu : MonoBehaviour
{
    [Header("Bindings")]
    public Button restartButton;
    public Button mainMenuButton;
    public Button quitButton;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI infoText;

    private LevelManager levelManager;

    public void Bind(LevelManager manager)
    {
        levelManager = manager;
        WireUp();
    }

    void Awake()
    {
        WireUp();
    }

    void WireUp()
    {
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(() => { Time.timeScale = 1f; levelManager?.RestartLevel(); });
        }
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(() => { Time.timeScale = 1f; levelManager?.LoadMainMenu(); });
        }
        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(() => {
                Time.timeScale = 1f;
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            });
        }
    }
}


