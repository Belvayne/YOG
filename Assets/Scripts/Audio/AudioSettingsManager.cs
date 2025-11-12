using UnityEngine;
using UnityEngine.Audio;

public class AudioSettingsManager : MonoBehaviour
{
    public static AudioSettingsManager Instance { get; private set; }

    [Header("Mixer & Exposed Params")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private string masterParam = "MasterVol";
    [SerializeField] private string musicParam = "MusicVol";
    [SerializeField] private string sfxParam = "SFXVol";
    [SerializeField] private string uiParam = "UIVol";

    const string MasterKey = "vol_master";
    const string MusicKey = "vol_music";
    const string SfxKey = "vol_sfx";
    const string UiKey = "vol_ui";

    // Defaults (linear 0..1)
    [Range(0, 1)] public float defaultMaster = 1f;
    [Range(0, 1)] public float defaultMusic = 0.8f;
    [Range(0, 1)] public float defaultSfx = 0.8f;
    [Range(0, 1)] public float defaultUi = 0.8f;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Verify mixer is assigned
        if (mixer == null)
        {
            Debug.LogError("AudioSettingsManager: AudioMixer is not assigned! Please assign it in the Inspector.");
            return;
        }
        
        LoadAndApply();
    }

    public void SetMaster(float v) => SetVolume(masterParam, MasterKey, v);
    public void SetMusic(float v) => SetVolume(musicParam, MusicKey, v);
    public void SetSfx(float v) => SetVolume(sfxParam, SfxKey, v);
    public void SetUi(float v) => SetVolume(uiParam, UiKey, v);

    public float GetMaster() => PlayerPrefs.GetFloat(MasterKey, defaultMaster);
    public float GetMusic() => PlayerPrefs.GetFloat(MusicKey, defaultMusic);
    public float GetSfx() => PlayerPrefs.GetFloat(SfxKey, defaultSfx);
    public float GetUi() => PlayerPrefs.GetFloat(UiKey, defaultUi);

    void SetVolume(string mixerParam, string prefKey, float linear)
    {
        if (mixer == null)
        {
            Debug.LogError($"AudioSettingsManager: Cannot set {mixerParam} - AudioMixer is null!");
            return;
        }
        
        // Save to PlayerPrefs
        PlayerPrefs.SetFloat(prefKey, linear);
        
        // Convert to decibels and apply to mixer
        float db = VolumeUtils.LinearToDecibels(linear);
        bool success = mixer.SetFloat(mixerParam, db);
        
        if (!success)
        {
            Debug.LogError($"AudioSettingsManager: Failed to set mixer parameter '{mixerParam}'. Make sure it's exposed in the AudioMixer!");
        }
        else
        {
            Debug.Log($"AudioSettingsManager: Set {mixerParam} to {linear:F2} ({db:F2} dB)");
        }
    }

    void LoadAndApply()
    {
        Debug.Log("AudioSettingsManager: Loading and applying audio settings...");
        SetMaster(GetMaster());
        SetMusic(GetMusic());
        SetSfx(GetSfx());
        SetUi(GetUi());
    }
}
