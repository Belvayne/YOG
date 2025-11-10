using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Splines; // only if you plan to load a scene
using UnityEngine.Timeline;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    public AudioSource clickSource;   // assign in Inspector
    public AudioClip clickClip;       // assign in Inspector

    private Button characterButton;
    private Button startButton;
    private Button settingsButton;
    private Button howtoplayButton;
    private Button creditsButton;
    private Button quitButton;
    private UIDocument UIDocument;

    [Header("Character Selector")]

    public Transform characterSpawnpoint;
    public GameObject selectedCharacter;
    public GameObject selectedPrefab;

    [SerializeField] private GameObject[] characterPrefabs;

    [SerializeField] private GameObject GuraPrefab;
    [SerializeField] private GameObject InaPrefab;
    [SerializeField] private GameObject AmePrefab;
    [SerializeField] private GameObject KiaraPrefab;
    [SerializeField] private GameObject CalliePrefab;

    private Button GuraButton;
    private Button InaButton;
    private Button AmeButton;
    private Button KiaraButton;
    private Button CallieButton;
    private Button confirmButton;

    private Button nextButton;
    private Button previousButton;
    private RadioButton[] radioButtons;

    private Label characterLabel;
    private Label titleLabel;

    [Header("Animation Setup")]
    [SerializeField] private PlayableDirector timelineDirector;
    [SerializeField] private TimelineAsset enterTimeline;
    [SerializeField] private TimelineAsset exitTimeline;

    [Header("UXML Assets")]
    public VisualTreeAsset mainMenuUXML;
    public VisualTreeAsset characterSelectUXML;
    public VisualTreeAsset sideScrollSelectUXML;
    public VisualTreeAsset settingsUXML;
    public VisualTreeAsset HowToPlayUXML;
    public VisualTreeAsset creditsUXML;

    void Start()
    {
        // Access root of UIDocument
        UIDocument = GetComponent<UIDocument>();
        RebindMainMenuButtons();

        // Set selected character to Gura by default
        selectedCharacter = Instantiate(GuraPrefab, characterSpawnpoint);
        selectedPrefab = GuraPrefab;
        GameDataManager.Instance.selectedCharacterPrefab = selectedPrefab;
    }

    private void RebindMainMenuButtons()
    {
        var root = UIDocument.rootVisualElement;

        // Get buttons by their names from your UXML
        characterButton = root.Q<Button>("CharacterSelectButton");
        startButton = root.Q<Button>("StartButton");
        settingsButton = root.Q<Button>("SettingsButton");
        howtoplayButton = root.Q<Button>("HowToPlayButton");
        creditsButton = root.Q<Button>("CreditsButton");
        quitButton = root.Q<Button>("QuitButton");

        // Add click event listeners
        characterButton.clicked += OnCharacterClicked;
        startButton.clicked += OnStartClicked;
        settingsButton.clicked += OnSettingsClicked;
        howtoplayButton.clicked += OnHowToPlayClicked;
        creditsButton.clicked += OnCreditsClicked;
        quitButton.clicked += OnQuitClicked;
    }

    void PlayClick()
    {
        clickSource.PlayOneShot(clickClip);
    }

    //void OnCharacterClicked()
    //{
    //    PlayClick();
    //    Debug.Log("Character Select Clicked!");
    //    if (timelineDirector != null)
    //    {
    //        timelineDirector.playableAsset = enterTimeline;
    //        timelineDirector.Play();
    //    }
    //    if (UIDocument != null && sideScrollSelectUXML != null)
    //    {
    //        UIDocument.visualTreeAsset = sideScrollSelectUXML;

    //        // Get the new root
    //        var newRoot = UIDocument.rootVisualElement;

    //        // Query buttons by their names in the new UXML
    //        nextButton = newRoot.Q<Button>("NextButton");
    //        previousButton = newRoot.Q<Button>("PreviousButton");
    //        confirmButton = newRoot.Q<Button>("CharacterConfirmButton");

    //        radioButtons = new RadioButton[characterPrefabs.Length];
    //        radioButtons[0] = newRoot.Q<RadioButton>("KiaraRadio");
    //        radioButtons[1] = newRoot.Q<RadioButton>("CallieRadio");
    //        radioButtons[2] = newRoot.Q<RadioButton>("GuraRadio");
    //        radioButtons[3] = newRoot.Q<RadioButton>("InaRadio");
    //        radioButtons[4] = newRoot.Q<RadioButton>("AmeRadio");

    //        // Optionally: make sure only the current one is checked at the start
    //        UpdateSelectedRadio(selectedPrefab);

    //        // Query labels
    //        characterLabel = newRoot.Q<Label>("CharacterName");
    //        titleLabel = newRoot.Q<Label>("CharacterTitle");

    //        var identity = selectedCharacter.GetComponent<CharacterIdentity>();
    //        if (identity != null && identity.data != null)
    //        {
    //            characterLabel.text = identity.data.characterName;
    //            titleLabel.text = identity.data.characterTitle;
    //        }

    //        // Assign click handlers
    //        nextButton.clicked += () => ScrollCharacter(selectedPrefab, true);
    //        previousButton.clicked += () => ScrollCharacter(selectedPrefab, false);

    //        confirmButton.clicked += () =>
    //        {
    //            PlayClick();
    //            Debug.Log("Character Confirmed!");
    //            UIDocument.visualTreeAsset = mainMenuUXML;

    //            var root = UIDocument.rootVisualElement;

    //            if (timelineDirector != null)
    //            {
    //                timelineDirector.playableAsset = exitTimeline;
    //                timelineDirector.Play();
    //            }

    //        RebindMainMenuButtons();
    
    //        };
    //    }
    //}

    void OnCharacterClicked()
    {
        PlayClick();
        Debug.Log("Character Select Clicked!");
        if (timelineDirector != null)
        {
            timelineDirector.playableAsset = enterTimeline;
            timelineDirector.Play();
        }
        if (UIDocument != null && characterSelectUXML != null)
        {
            UIDocument.visualTreeAsset = characterSelectUXML;

            // Get the new root
            var newRoot = UIDocument.rootVisualElement;

            // Query buttons by their names in the new UXML
            GuraButton = newRoot.Q<Button>("GuraButton");
            InaButton = newRoot.Q<Button>("InaButton");
            AmeButton = newRoot.Q<Button>("AmeButton");
            KiaraButton = newRoot.Q<Button>("KiaraButton");
            CallieButton = newRoot.Q<Button>("CallieButton");
            confirmButton = newRoot.Q<Button>("CharacterConfirmButton");

            // Query labels
            characterLabel = newRoot.Q<Label>("CharacterName");
            titleLabel = newRoot.Q<Label>("CharacterTitle");

            var identity = selectedCharacter.GetComponent<CharacterIdentity>();
            if (identity != null && identity.data != null)
            {
                characterLabel.text = identity.data.characterName;
                titleLabel.text = identity.data.characterTitle;
            }

            // Assign click handlers
            GuraButton.clicked += () => OnCharacterSelected(GuraPrefab);
            InaButton.clicked += () => OnCharacterSelected(InaPrefab);
            AmeButton.clicked += () => OnCharacterSelected(AmePrefab);
            KiaraButton.clicked += () => OnCharacterSelected(KiaraPrefab);
            CallieButton.clicked += () => OnCharacterSelected(CalliePrefab);
            confirmButton.clicked += () =>
            {
                PlayClick();
                Debug.Log("Character Confirmed!");
                UIDocument.visualTreeAsset = mainMenuUXML;

                var root = UIDocument.rootVisualElement;

                if (timelineDirector != null)
                {
                    timelineDirector.playableAsset = exitTimeline;
                    timelineDirector.Play();
                }

                RebindMainMenuButtons();
            };
        }
    }

    void OnStartClicked()
    {
        PlayClick();
        Debug.Log("Start Game Clicked!");
        SceneManager.LoadScene("EndlessMode");
    }

    void OnQuitClicked()
    {
        PlayClick();
        Debug.Log("Quit Game Clicked!");
        Application.Quit();
    }

    void OnSettingsClicked()
    {
        PlayClick();
        Debug.Log("Settings Clicked!");
        if (UIDocument != null && settingsUXML != null)
        {
            UIDocument.visualTreeAsset = settingsUXML;
            
            var newRoot = UIDocument.rootVisualElement;
            
            // Query for a back/close button in the settings UI
            var backButton = newRoot.Q<Button>("BackButton");
            if (backButton != null)
            {
                backButton.clicked += () =>
                {
                    PlayClick();
                    UIDocument.visualTreeAsset = mainMenuUXML;
                    RebindMainMenuButtons();
                };
            }
        }
    }

    void OnHowToPlayClicked()
    {
        PlayClick();
        Debug.Log("How To Play Clicked!");
        if (UIDocument != null && HowToPlayUXML != null)
        {
            UIDocument.visualTreeAsset = HowToPlayUXML;
            
            var newRoot = UIDocument.rootVisualElement;
            
            // Query for a back/close button in the how to play UI
            var backButton = newRoot.Q<Button>("BackButton");
            if (backButton != null)
            {
                backButton.clicked += () =>
                {
                    PlayClick();
                    UIDocument.visualTreeAsset = mainMenuUXML;
                    RebindMainMenuButtons();
                };
            }
        }
    }

    void OnCreditsClicked()
    {
        PlayClick();
        Debug.Log("Credits Clicked!");
        if (UIDocument != null && creditsUXML != null)
        {
            UIDocument.visualTreeAsset = creditsUXML;
            
            var newRoot = UIDocument.rootVisualElement;
            
            // Query for a back/close button in the credits UI
            var backButton = newRoot.Q<Button>("BackButton");
            if (backButton != null)
            {
                backButton.clicked += () =>
                {
                    PlayClick();
                    UIDocument.visualTreeAsset = mainMenuUXML;
                    RebindMainMenuButtons();
                };
            }
        }
    }

    //void ScrollCharacter(GameObject currentPrefab, bool isNext)
    //{
    //    PlayClick();
    //    Debug.Log("Scroll Character Clicked!");

    //    int currentIndex = System.Array.IndexOf(characterPrefabs, currentPrefab);
    //    int newIndex;
    //    if (isNext)
    //    {
    //        newIndex = (currentIndex + 1) % characterPrefabs.Length;
    //    }
    //    else
    //    {
    //        newIndex = (currentIndex - 1 + characterPrefabs.Length) % characterPrefabs.Length;
    //    }
    //    GameObject newPrefab = characterPrefabs[newIndex];
    //    Destroy(selectedCharacter);
    //    selectedCharacter = Instantiate(newPrefab, characterSpawnpoint);
    //    selectedPrefab = newPrefab;
    //    var identity = newPrefab.GetComponent<CharacterIdentity>();
    //    if (identity != null && identity.data != null)
    //    {
    //        characterLabel.text = identity.data.characterName;
    //        titleLabel.text = identity.data.characterTitle;
    //    }

    //    UpdateSelectedRadio(selectedPrefab);
    //}

    //void UpdateSelectedRadio(GameObject currentPrefab)
    //{
    //    int currentIndex = System.Array.IndexOf(characterPrefabs, currentPrefab);

    //    for (int i = 0; i < radioButtons.Length; i++)
    //    {
    //        // Avoid null references if buttons aren't found
    //        if (radioButtons[i] != null)
    //            radioButtons[i].value = (i == currentIndex);
    //    }
    //}


    void OnCharacterSelected(GameObject prefab)
    {
        Debug.Log($"Character selected: {prefab}");
        if (prefab != selectedPrefab && prefab != null)
        {
            PlayClick();
            Destroy(selectedCharacter);
            selectedCharacter = Instantiate(prefab, characterSpawnpoint);
            selectedPrefab = prefab;
            GameDataManager.Instance.selectedCharacterPrefab = selectedPrefab;

            var identity = prefab.GetComponent<CharacterIdentity>();
            if (identity != null && identity.data != null)
            {
                characterLabel.text = identity.data.characterName;
                titleLabel.text = identity.data.characterTitle;
            }
        }
    }
}
