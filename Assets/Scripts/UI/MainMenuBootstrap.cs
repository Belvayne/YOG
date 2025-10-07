using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public static class MainMenuBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void WireMainMenuIfPresent()
    {
        var uiDocument = Object.FindObjectOfType<UIDocument>();
        if (uiDocument == null) return;

        // Delay wiring until the panel is fully attached to ensure elements exist
        var root = uiDocument.rootVisualElement;
        if (root == null) return;

        // Make sure menu is interactive after returning from gameplay
        Time.timeScale = 1f;
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = UnityEngine.CursorLockMode.None;

        root.UnregisterCallback<AttachToPanelEvent>(OnAttach);
        root.RegisterCallback<AttachToPanelEvent>(OnAttach);

        // In case it's already attached, also schedule a next-frame wire
        root.schedule.Execute(() => WireButtons(root)).StartingIn(0);
    }

    private static void OnAttach(AttachToPanelEvent evt)
    {
        var root = evt.target as VisualElement;
        if (root != null)
        {
            WireButtons(root);
        }
    }

    private static void WireButtons(VisualElement root)
    {
        var startButton = root.Q<Button>("StartButton");
        var quitButton = root.Q<Button>("QuitButton");

        if (startButton != null)
        {
            startButton.clicked -= OnStartClicked;
            startButton.clicked += OnStartClicked;
            startButton.Focus();
        }

        if (quitButton != null)
        {
            quitButton.clicked -= OnQuitClicked;
            quitButton.clicked += OnQuitClicked;
        }
    }

    private static void OnStartClicked()
    {
        // Prefer SceneLoader if present
        var sceneLoader = Object.FindObjectOfType<SceneLoader>();
        if (sceneLoader != null)
        {
            sceneLoader.LoadGameScene();
            return;
        }
        SceneManager.LoadScene("GameScene");
    }

    private static void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}


