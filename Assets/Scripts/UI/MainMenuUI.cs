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

    void Start()
    {
        // Access root of UIDocument
        UIDocument = GetComponent<UIDocument>();
        var root = UIDocument.rootVisualElement;

        // Get buttons by their names from your UXML
        characterButton = root.Q<Button>("CharacterSelectButton");
        startButton = root.Q<Button>("StartButton");
        quitButton = root.Q<Button>("QuitButton");


        // Add click event listeners
        characterButton.clicked += OnCharacterClicked;
        startButton.clicked += OnStartClicked;
        quitButton.clicked += OnQuitClicked;

        // Set selected character to Gura by default
        selectedCharacter = Instantiate(GuraPrefab, characterSpawnpoint);
        selectedPrefab = GuraPrefab;
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

    //            // Get buttons by their names from your UXML
    //            characterButton = root.Q<Button>("CharacterSelectButton");
    //            startButton = root.Q<Button>("StartButton");
    //            quitButton = root.Q<Button>("QuitButton");


    //            // Add click event listeners
    //            characterButton.clicked += OnCharacterClicked;
    //            startButton.clicked += OnStartClicked;
    //            quitButton.clicked += OnQuitClicked;
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

                // Get buttons by their names from your UXML
                characterButton = root.Q<Button>("CharacterSelectButton");
                startButton = root.Q<Button>("StartButton");
                quitButton = root.Q<Button>("QuitButton");


                // Add click event listeners
                characterButton.clicked += OnCharacterClicked;
                startButton.clicked += OnStartClicked;
                quitButton.clicked += OnQuitClicked;
            };
        }
    }

    void OnStartClicked()
    {
        PlayClick();
        Debug.Log("Start Game Clicked!");
        SceneManager.LoadScene("GameScene");
    }

    void OnQuitClicked()
    {
        PlayClick();
        Debug.Log("Quit Game Clicked!");
        Application.Quit();
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

            var identity = prefab.GetComponent<CharacterIdentity>();
            if (identity != null && identity.data != null)
            {
                characterLabel.text = identity.data.characterName;
                titleLabel.text = identity.data.characterTitle;
            }
        }
    }
}
