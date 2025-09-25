using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SimpleMainMenu : MonoBehaviour
{
    [Header("Scene")] 
    public string gameSceneName = "GameScene";

    [Header("Optional: Assign existing UI")] 
    public Button startButton;
    public Button quitButton;

    void Awake()
    {
        EnsureEventSystem();
    }

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // If buttons not assigned, try to find by name
        if (startButton == null) startButton = GameObject.Find("StartButton")?.GetComponent<Button>();
        if (quitButton == null) quitButton = GameObject.Find("QuitButton")?.GetComponent<Button>();

        // If still missing, auto-build a very simple UI
        if (startButton == null || quitButton == null)
        {
            BuildMinimalUI();
        }

        // Wire up listeners
        if (startButton != null)
        {
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(StartGame);
        }
        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }
    }

    private void BuildMinimalUI()
    {
        // Create Canvas if none exists in scene
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            var canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        // Panel
        var panelGO = new GameObject("Panel");
        panelGO.transform.SetParent(canvas.transform, false);
        var panelRT = panelGO.AddComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0.5f, 0.5f);
        panelRT.anchorMax = new Vector2(0.5f, 0.5f);
        panelRT.sizeDelta = new Vector2(400, 220);
        panelRT.anchoredPosition = Vector2.zero;
        var panelImg = panelGO.AddComponent<Image>();
        panelImg.color = new Color(0f, 0f, 0f, 0.6f);

        // Helper to make a button
        Button MakeButton(string name, string label, Vector2 pos)
        {
            var btnGO = new GameObject(name);
            btnGO.transform.SetParent(panelGO.transform, false);
            var rt = btnGO.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(260, 60);
            rt.anchoredPosition = pos;
            var img = btnGO.AddComponent<Image>();
            img.color = new Color(1f, 1f, 1f, 0.2f);
            var btn = btnGO.AddComponent<Button>();

            var textGO = new GameObject("Text");
            textGO.transform.SetParent(btnGO.transform, false);
            var trt = textGO.AddComponent<RectTransform>();
            trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one;
            trt.offsetMin = Vector2.zero; trt.offsetMax = Vector2.zero;
            var txt = textGO.AddComponent<UnityEngine.UI.Text>();
            txt.text = label;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.fontSize = 28;
            txt.color = Color.white;
            txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            return btn;
        }

        startButton = MakeButton("StartButton", "Start Game", new Vector2(0, 50));
        quitButton = MakeButton("QuitButton", "Quit", new Vector2(0, -30));
    }
}


