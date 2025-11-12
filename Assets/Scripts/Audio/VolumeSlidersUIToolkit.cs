using UnityEngine;
using UnityEngine.UIElements;

public class VolumeSlidersUIToolkit : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;

    Slider _master, _music, _sfx, _ui;
    Button _back;

    void Awake()
    {
        if (uiDocument == null) uiDocument = GetComponent<UIDocument>();
        
        if (uiDocument == null)
        {
            Debug.LogError("VolumeSlidersUIToolkit: UIDocument is missing!");
        }
    }

    void OnEnable()
    {
        if (uiDocument == null)
        {
            Debug.LogError("VolumeSlidersUIToolkit: Cannot initialize - UIDocument is null!");
            return;
        }
        
        if (AudioSettingsManager.Instance == null)
        {
            Debug.LogError("VolumeSlidersUIToolkit: AudioSettingsManager.Instance is null! Make sure AudioSettingsManager exists in the scene.");
            return;
        }
        
        var root = uiDocument.rootVisualElement;

        _master = root.Q<Slider>("MasterSlider");
        _music = root.Q<Slider>("MusicSlider");
        _sfx = root.Q<Slider>("SFXSlider");
        _ui = root.Q<Slider>("UiSlider");
        _back = root.Q<Button>("BackButton");
        
        Debug.Log($"Sliders found - Master: {_master != null}, Music: {_music != null}, SFX: {_sfx != null}, UI: {_ui != null}");

        var mgr = AudioSettingsManager.Instance;

        if (_master != null)
            _master.SetValueWithoutNotify(mgr.GetMaster() * 100f);
        else
            Debug.LogError("VolumeSlidersUIToolkit: MasterSlider not found in UI!");
            
        if (_music != null)
            _music.SetValueWithoutNotify(mgr.GetMusic() * 100f);
        else
            Debug.LogError("VolumeSlidersUIToolkit: MusicSlider not found in UI!");
            
        if (_sfx != null)
            _sfx.SetValueWithoutNotify(mgr.GetSfx() * 100f);
        else
            Debug.LogError("VolumeSlidersUIToolkit: SFXSlider not found in UI!");
            
        if (_ui != null)
            _ui.SetValueWithoutNotify(mgr.GetUi() * 100f);
        else
            Debug.LogError("VolumeSlidersUIToolkit: UiSlider not found in UI!");

        _master?.RegisterValueChangedCallback(evt => {
            mgr.SetMaster(evt.newValue / 100f);
            Debug.Log($"Master volume changed to: {evt.newValue}%");
        });
        
        _music?.RegisterValueChangedCallback(evt => {
            mgr.SetMusic(evt.newValue / 100f);
            Debug.Log($"Music volume changed to: {evt.newValue}%");
        });
        
        _sfx?.RegisterValueChangedCallback(evt => {
            mgr.SetSfx(evt.newValue / 100f);
            Debug.Log($"SFX volume changed to: {evt.newValue}%");
        });
        
        _ui?.RegisterValueChangedCallback(evt => {
            mgr.SetUi(evt.newValue / 100f);
            Debug.Log($"UI volume changed to: {evt.newValue}%");
        });

        _back?.RegisterCallback<ClickEvent>(_ => GoBackToMenu());
    }

    void GoBackToMenu()
    {
        Debug.Log("VolumeSlidersUIToolkit: Back button clicked");
        
        // Find the MainMenuUI and call its back method
        var mainMenuUI = FindFirstObjectByType<MainMenuUI>();
        if (mainMenuUI != null)
        {
            mainMenuUI.OnSettingsBackClicked();
        }
        else
        {
            Debug.LogError("VolumeSlidersUIToolkit: Could not find MainMenuUI!");
        }
    }
}
