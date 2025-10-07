using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : EditorWindow
{
    private static readonly Color Accent = new Color(0.27f, 0.82f, 1.00f); // light cyan
    private static readonly Color AccentDark = new Color(0.10f, 0.65f, 0.95f);

    [MenuItem("Window/UI Toolkit/Main Menu Preview")]
    public static void ShowExample()
    {
        MainMenu wnd = GetWindow<MainMenu>();
        wnd.titleContent = new GUIContent("Main Menu");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        root.Clear();

        // Layout: center everything vertically and horizontally on a white background
        root.style.flexGrow = 1;
        root.style.backgroundColor = Color.white;
        root.style.justifyContent = Justify.Center;
        root.style.alignItems = Align.Center;

        // Container to stack title and buttons
        var column = new VisualElement();
        column.style.flexDirection = FlexDirection.Column;
        column.style.alignItems = Align.Center;
        column.style.paddingTop = 24;
        column.style.paddingBottom = 24;
        root.Add(column);

        // Title
        var title = new Label("CHIBILIVE: MASCOT MAYHEM!");
        title.style.unityTextAlign = TextAnchor.MiddleCenter;
        title.style.fontSize = 64;
        title.style.unityFontStyleAndWeight = FontStyle.Bold;
        title.style.color = new StyleColor(Accent);
        // Outline for the bubbly effect
        title.style.unityTextOutlineColor = new StyleColor(Color.white);
        title.style.unityTextOutlineWidth = 2;
        title.style.marginBottom = 40;
        column.Add(title);

        // Buttons container
        var buttons = new VisualElement();
        buttons.style.flexDirection = FlexDirection.Column;
        buttons.style.alignItems = Align.Center;
        column.Add(buttons);

        // Helper to build a rounded outlined button matching the mock
        Button MakeMenuButton(string text)
        {
            var btn = new Button { text = text };
            btn.style.width = 420;
            btn.style.height = 92;
            btn.style.marginTop = 10;
            btn.style.marginBottom = 10;
            btn.style.backgroundColor = Color.white;
            btn.style.borderTopWidth = 4;
            btn.style.borderRightWidth = 4;
            btn.style.borderBottomWidth = 4;
            btn.style.borderLeftWidth = 4;
            btn.style.borderTopColor = new StyleColor(Accent);
            btn.style.borderRightColor = new StyleColor(Accent);
            btn.style.borderBottomColor = new StyleColor(Accent);
            btn.style.borderLeftColor = new StyleColor(Accent);
            btn.style.borderTopLeftRadius = 46;
            btn.style.borderTopRightRadius = 46;
            btn.style.borderBottomLeftRadius = 46;
            btn.style.borderBottomRightRadius = 46;
            btn.style.color = new StyleColor(Accent);
            btn.style.fontSize = 28;
            btn.style.unityFontStyleAndWeight = FontStyle.Bold;
            btn.style.unityTextAlign = TextAnchor.MiddleCenter;

            // Hover feedback
            btn.RegisterCallback<MouseEnterEvent>(_ =>
            {
                btn.style.backgroundColor = new StyleColor(new Color(0.95f, 1f, 1f));
                btn.style.borderTopColor = new StyleColor(AccentDark);
                btn.style.borderRightColor = new StyleColor(AccentDark);
                btn.style.borderBottomColor = new StyleColor(AccentDark);
                btn.style.borderLeftColor = new StyleColor(AccentDark);
                btn.style.color = new StyleColor(AccentDark);
            });

            btn.RegisterCallback<MouseLeaveEvent>(_ =>
            {
                btn.style.backgroundColor = new StyleColor(Color.white);
                btn.style.borderTopColor = new StyleColor(Accent);
                btn.style.borderRightColor = new StyleColor(Accent);
                btn.style.borderBottomColor = new StyleColor(Accent);
                btn.style.borderLeftColor = new StyleColor(Accent);
                btn.style.color = new StyleColor(Accent);
            });

            return btn;
        }

        var startBtn = MakeMenuButton("START GAME");
        startBtn.clicked += () => { EditorApplication.isPlaying = true; };
        buttons.Add(startBtn);

        var quitBtn = MakeMenuButton("QUIT GAME");
        quitBtn.clicked += () => { EditorApplication.Exit(0); };
        buttons.Add(quitBtn);
    }
}
