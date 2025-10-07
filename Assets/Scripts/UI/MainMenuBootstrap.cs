using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public static class MainMenuBootstrap
{
    private static bool _subscribed;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        if (_subscribed) return;
        SceneManager.sceneLoaded += OnSceneLoaded;
        _subscribed = true;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        WireMainMenuIfPresent();
    }

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

        // Ensure only one attach handler
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

        if (startButton == null && quitButton == null)
        {
            // Try common alternative names/cases
            startButton = root.Q<Button>(name: "Start", className: null) ?? root.Q<Button>("startButton");
            quitButton = root.Q<Button>(name: "Quit", className: null) ?? root.Q<Button>("quitButton");
        }

        if (startButton != null)
        {
            startButton.clicked -= OnStartClicked;
            startButton.clicked += OnStartClicked;
            startButton.Focus();
            SetupHighlight(startButton);
        }

        if (quitButton != null)
        {
            quitButton.clicked -= OnQuitClicked;
            quitButton.clicked += OnQuitClicked;
            SetupHighlight(quitButton);
        }
    }

    private static void SetupHighlight(Button button)
    {
        button.focusable = true;

        var originalBg = button.resolvedStyle.backgroundColor;
        var originalBorderTop = button.resolvedStyle.borderTopColor;
        var originalBorderRight = button.resolvedStyle.borderRightColor;
        var originalBorderBottom = button.resolvedStyle.borderBottomColor;
        var originalBorderLeft = button.resolvedStyle.borderLeftColor;

        var label = button.Q<Label>();
        var originalLabelColor = label != null ? label.resolvedStyle.color : Color.white;

        void Apply(bool on)
        {
            if (on)
            {
                button.style.backgroundColor = new StyleColor(new Color(109f, 229f, 255f, 0.9f));
                var cyan = new Color(109f / 255f, 229f / 255f, 255f / 255f, 1f);
                button.style.borderTopColor = cyan;
                button.style.borderRightColor = cyan;
                button.style.borderBottomColor = cyan;
                button.style.borderLeftColor = cyan;
                if (label != null) label.style.color = cyan;
            }
            else
            {
                button.style.backgroundColor = new StyleColor(originalBg);
                button.style.borderTopColor = new StyleColor(originalBorderTop);
                button.style.borderRightColor = new StyleColor(originalBorderRight);
                button.style.borderBottomColor = new StyleColor(originalBorderBottom);
                button.style.borderLeftColor = new StyleColor(originalBorderLeft);
                if (label != null) label.style.color = new StyleColor(originalLabelColor);
            }
        }

        button.RegisterCallback<MouseEnterEvent>(_ => Apply(true));
        button.RegisterCallback<MouseLeaveEvent>(_ => Apply(false));
        button.RegisterCallback<FocusInEvent>(_ => Apply(true));
        button.RegisterCallback<FocusOutEvent>(_ => Apply(false));
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


