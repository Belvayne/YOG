using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuUISetup : MonoBehaviour
{
    [Header("Auto Setup")]
    public bool setupOnStart = true;
    public bool createMenuUI = true;
    public bool createGameUI = true;
    public bool createPauseUI = true;
    public bool createGameOverUI = true;
    public bool createLevelCompleteUI = true;
    
    [Header("UI Settings")]
    public string gameTitle = "YOG Game";
    public Color buttonColor = Color.white;
    public Color textColor = Color.white;
    public Font buttonFont;
    public Font textFont;
    
    [Header("Canvas Settings")]
    public CanvasScaler.ScaleMode scaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
    public Vector2 referenceResolution = new Vector2(1920, 1080);
    
    private Canvas canvas;
    private MenuManager menuManager;
    
    void Start()
    {
        if (setupOnStart)
        {
            SetupMenuUI();
        }
    }
    
    [ContextMenu("Setup Menu UI")]
    public void SetupMenuUI()
    {
        Debug.Log("Setting up menu UI...");
        
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
        
        // Get or create MenuManager
        menuManager = GetComponent<MenuManager>();
        if (menuManager == null)
        {
            menuManager = gameObject.AddComponent<MenuManager>();
        }
        
        // Create UI panels
        if (createMenuUI) CreateStartMenu();
        if (createGameUI) CreateGameUI();
        if (createPauseUI) CreatePauseMenu();
        if (createGameOverUI) CreateGameOverMenu();
        if (createLevelCompleteUI) CreateLevelCompleteMenu();
        
        Debug.Log("Menu UI setup complete!");
    }
    
    void CreateStartMenu()
    {
        // Create start menu panel
        GameObject startPanel = CreatePanel("StartMenuPanel", new Vector2(0, 0), new Vector2(1, 1));
        menuManager.startMenuPanel = startPanel;
        
        // Create title
        GameObject title = CreateText("Title", "YOG Game", new Vector2(0, 200), new Vector2(400, 100), 48);
        title.transform.SetParent(startPanel.transform, false);
        menuManager.titleText = title.GetComponent<TextMeshProUGUI>();
        
        // Create start button
        GameObject startButton = CreateButton("StartButton", "Start Game", new Vector2(0, 50), new Vector2(200, 50));
        startButton.transform.SetParent(startPanel.transform, false);
        menuManager.startGameButton = startButton.GetComponent<Button>();
        
        // Create quit button
        GameObject quitButton = CreateButton("QuitButton", "Quit Game", new Vector2(0, -50), new Vector2(200, 50));
        quitButton.transform.SetParent(startPanel.transform, false);
        menuManager.quitGameButton = quitButton.GetComponent<Button>();
    }
    
    void CreateGameUI()
    {
        // Create game UI panel
        GameObject gamePanel = CreatePanel("GameUIPanel", new Vector2(0, 0), new Vector2(1, 1));
        menuManager.gameUIPanel = gamePanel;
        
        // This will be populated by your existing GameUI script
        GameUI gameUI = FindObjectOfType<GameUI>();
        if (gameUI != null)
        {
            gameUI.transform.SetParent(gamePanel.transform, false);
        }
    }
    
    void CreatePauseMenu()
    {
        // Create pause menu panel
        GameObject pausePanel = CreatePanel("PauseMenuPanel", new Vector2(0, 0), new Vector2(1, 1));
        pausePanel.SetActive(false);
        menuManager.pauseMenuPanel = pausePanel;
        
        // Create pause title
        GameObject pauseTitle = CreateText("PauseTitle", "Game Paused", new Vector2(0, 100), new Vector2(400, 80), 36);
        pauseTitle.transform.SetParent(pausePanel.transform, false);
        
        // Create resume button
        GameObject resumeButton = CreateButton("ResumeButton", "Resume", new Vector2(0, 20), new Vector2(200, 50));
        resumeButton.transform.SetParent(pausePanel.transform, false);
        menuManager.resumeButton = resumeButton.GetComponent<Button>();
        
        // Create main menu button
        GameObject mainMenuButton = CreateButton("MainMenuButton", "Main Menu", new Vector2(0, -40), new Vector2(200, 50));
        mainMenuButton.transform.SetParent(pausePanel.transform, false);
        menuManager.mainMenuPauseButton = mainMenuButton.GetComponent<Button>();
        
        // Create quit button
        GameObject quitButton = CreateButton("QuitPauseButton", "Quit", new Vector2(0, -100), new Vector2(200, 50));
        quitButton.transform.SetParent(pausePanel.transform, false);
        menuManager.quitPauseButton = quitButton.GetComponent<Button>();
    }
    
    void CreateGameOverMenu()
    {
        // Create game over panel
        GameObject gameOverPanel = CreatePanel("GameOverPanel", new Vector2(0, 0), new Vector2(1, 1));
        gameOverPanel.SetActive(false);
        menuManager.gameOverPanel = gameOverPanel;
        
        // Create game over title
        GameObject gameOverTitle = CreateText("GameOverTitle", "Game Over!", new Vector2(0, 100), new Vector2(400, 80), 36);
        gameOverTitle.transform.SetParent(gameOverPanel.transform, false);
        menuManager.gameOverText = gameOverTitle.GetComponent<TextMeshProUGUI>();
        
        // Create final score text
        GameObject finalScore = CreateText("FinalScore", "Final Score: 0", new Vector2(0, 40), new Vector2(400, 40), 24);
        finalScore.transform.SetParent(gameOverPanel.transform, false);
        menuManager.finalScoreText = finalScore.GetComponent<TextMeshProUGUI>();
        
        // Create restart button
        GameObject restartButton = CreateButton("RestartButton", "Restart", new Vector2(0, -20), new Vector2(200, 50));
        restartButton.transform.SetParent(gameOverPanel.transform, false);
        menuManager.restartButton = restartButton.GetComponent<Button>();
        
        // Create main menu button
        GameObject mainMenuButton = CreateButton("MainMenuGameOverButton", "Main Menu", new Vector2(0, -80), new Vector2(200, 50));
        mainMenuButton.transform.SetParent(gameOverPanel.transform, false);
        menuManager.mainMenuButton = mainMenuButton.GetComponent<Button>();
        
        // Create quit button
        GameObject quitButton = CreateButton("QuitGameOverButton", "Quit", new Vector2(0, -140), new Vector2(200, 50));
        quitButton.transform.SetParent(gameOverPanel.transform, false);
        menuManager.quitButton = quitButton.GetComponent<Button>();
    }
    
    void CreateLevelCompleteMenu()
    {
        // Create level complete panel
        GameObject levelCompletePanel = CreatePanel("LevelCompletePanel", new Vector2(0, 0), new Vector2(1, 1));
        levelCompletePanel.SetActive(false);
        menuManager.levelCompletePanel = levelCompletePanel;
        
        // Create level complete title
        GameObject levelCompleteTitle = CreateText("LevelCompleteTitle", "Level Complete!", new Vector2(0, 100), new Vector2(400, 80), 36);
        levelCompleteTitle.transform.SetParent(levelCompletePanel.transform, false);
        menuManager.levelCompleteText = levelCompleteTitle.GetComponent<TextMeshProUGUI>();
        
        // Create completion time text
        GameObject completionTime = CreateText("CompletionTime", "Time: 0.0s | Kills: 0", new Vector2(0, 40), new Vector2(400, 40), 24);
        completionTime.transform.SetParent(levelCompletePanel.transform, false);
        menuManager.completionTimeText = completionTime.GetComponent<TextMeshProUGUI>();
        
        // Create next level button
        GameObject nextLevelButton = CreateButton("NextLevelButton", "Next Level", new Vector2(0, -20), new Vector2(200, 50));
        nextLevelButton.transform.SetParent(levelCompletePanel.transform, false);
        menuManager.nextLevelButton = nextLevelButton.GetComponent<Button>();
        
        // Create main menu button
        GameObject mainMenuButton = CreateButton("MainMenuCompleteButton", "Main Menu", new Vector2(0, -80), new Vector2(200, 50));
        mainMenuButton.transform.SetParent(levelCompletePanel.transform, false);
        menuManager.mainMenuCompleteButton = mainMenuButton.GetComponent<Button>();
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
        
        Image image = panel.AddComponent<Image>();
        image.color = new Color(0, 0, 0, 0.8f);
        
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
        textComponent.fontSize = 24;
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
