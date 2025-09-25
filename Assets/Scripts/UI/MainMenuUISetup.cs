using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUISetup : MonoBehaviour
{
    [Header("Auto Setup")]
    public bool setupOnStart = true;
    public bool createBackground = true;
    
    [Header("UI Settings")]
    public string gameTitle = "YOG Game";
    public string gameVersion = "1.0.0";
    public Color buttonColor = Color.white;
    public Color textColor = Color.white;
    public Color backgroundColor = Color.black;
    public Font buttonFont;
    public Font textFont;
    
    [Header("Canvas Settings")]
    public CanvasScaler.ScaleMode scaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
    public Vector2 referenceResolution = new Vector2(1920, 1080);
    
    private Canvas canvas;
    private MainMenuManager menuManager;
    
    void Start()
    {
        if (setupOnStart)
        {
            SetupMainMenuUI();
        }
    }
    
    [ContextMenu("Setup Main Menu UI")]
    public void SetupMainMenuUI()
    {
        Debug.Log("Setting up main menu UI...");
        
        // Get or create canvas
        canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
        }
        
        // Add CanvasScaler
        CanvasScaler scaler = GetComponent<CanvasScaler>();
        if (scaler == null)
        {
            scaler = gameObject.AddComponent<CanvasScaler>();
        }
        scaler.uiScaleMode = scaleMode;
        scaler.referenceResolution = referenceResolution;
        
        // Add GraphicRaycaster
        GraphicRaycaster raycaster = GetComponent<GraphicRaycaster>();
        if (raycaster == null)
        {
            raycaster = gameObject.AddComponent<GraphicRaycaster>();
        }
        
        // Get or create MainMenuManager
        menuManager = GetComponent<MainMenuManager>();
        if (menuManager == null)
        {
            menuManager = gameObject.AddComponent<MainMenuManager>();
        }
        
        // Create UI elements
        CreateMainMenuUI();
        
        // Setup button listeners
        SetupButtonListeners();
        
        // Setup background
        if (createBackground)
        {
            SetupBackground();
        }
        
        Debug.Log("Main menu UI setup complete!");
    }
    
    void CreateMainMenuUI()
    {
        // Create main panel
        GameObject mainPanel = CreatePanel("MainPanel", new Vector2(0, 0), new Vector2(1, 1));
        
        // Create title
        GameObject title = CreateText("Title", gameTitle, new Vector2(0, 200), new Vector2(600, 120), 72);
        title.transform.SetParent(mainPanel.transform, false);
        menuManager.titleText = title.GetComponent<TextMeshProUGUI>();
        
        // Create version text
        GameObject version = CreateText("Version", $"Version {gameVersion}", new Vector2(0, 120), new Vector2(400, 40), 24);
        version.transform.SetParent(mainPanel.transform, false);
        menuManager.versionText = version.GetComponent<TextMeshProUGUI>();
        
        // Create start button
        GameObject startButton = CreateButton("StartButton", "Start Game", new Vector2(0, 20), new Vector2(300, 80));
        startButton.transform.SetParent(mainPanel.transform, false);
        menuManager.startGameButton = startButton.GetComponent<Button>();
        
        // Create quit button (moved up since no settings button)
        GameObject quitButton = CreateButton("QuitButton", "Quit Game", new Vector2(0, -80), new Vector2(300, 80));
        quitButton.transform.SetParent(mainPanel.transform, false);
        menuManager.quitGameButton = quitButton.GetComponent<Button>();
    }
    
    void SetupBackground()
    {
        // Set camera background color
        Camera.main.backgroundColor = backgroundColor;
        
        // Create background image
        GameObject background = new GameObject("Background");
        background.transform.SetParent(transform, false);
        
        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = backgroundColor;
        
        // Move background to back
        background.transform.SetAsFirstSibling();
        
        menuManager.backgroundImage = background;
    }
    
    void SetupButtonListeners()
    {
        // Setup button listeners
        if (menuManager.startGameButton != null)
        {
            menuManager.startGameButton.onClick.AddListener(menuManager.StartGame);
        }
        
        if (menuManager.quitGameButton != null)
        {
            menuManager.quitGameButton.onClick.AddListener(menuManager.QuitGame);
        }
        
        Debug.Log("Button listeners setup complete!");
    }
    
    GameObject CreatePanel(string name, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(transform, false);
        
        RectTransform rectTransform = panel.AddComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        
        return panel;
    }
    
    GameObject CreateButton(string name, string text, Vector2 position, Vector2 size)
    {
        GameObject button = new GameObject(name);
        
        RectTransform rectTransform = button.AddComponent<RectTransform>();
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = position;
        
        Image image = button.AddComponent<Image>();
        image.color = buttonColor;
        
        Button buttonComponent = button.AddComponent<Button>();
        
        // Create button text
        GameObject buttonText = new GameObject("Text");
        buttonText.transform.SetParent(button.transform, false);
        
        RectTransform textRect = buttonText.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI textComponent = buttonText.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = 32;
        textComponent.color = textColor;
        textComponent.alignment = TextAlignmentOptions.Center;
        
        return button;
    }
    
    GameObject CreateText(string name, string text, Vector2 position, Vector2 size, int fontSize)
    {
        GameObject textObj = new GameObject(name);
        
        RectTransform rectTransform = textObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = position;
        
        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.color = textColor;
        textComponent.alignment = TextAlignmentOptions.Center;
        
        return textObj;
    }
}
