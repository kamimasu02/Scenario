using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionData
{
    public class TextOptions
    {
        public float typeSpeed = 0.05f;
        public float autoSpeed = 0.5f;
    }

    public class AudioOptions
    {
        public float bgmVolume = 0.5f;
        public float sfxVolume = 0.5f;
        public float voiceVolume = 0.5f;
        public bool bgmMute = false;
        public bool sfxMute = false;
        public bool voiceMute = false;
    }

    public class ResolutionOptions
    {
        public int index = 0;
        public bool isFullScreen = false;
    }

    public TextOptions text = new TextOptions();
    public AudioOptions audio = new AudioOptions();
    public ResolutionOptions resolution = new ResolutionOptions();
    
}

public class OptionController : MonoBehaviour
{
    [SerializeField] GameObject audioBGMObject;
    [SerializeField] GameObject audioSFXObject;
    [SerializeField] GameObject dialogueObject;
    
    public static OptionController Instance { get; private set; }

    public delegate void OnVoiceOptionChange(float voiceVolume, bool voiceMute = false);
    public static event OnVoiceOptionChange VoiceOptionChange;

    public OptionData options = new OptionData();

    private Resolution[] _availableResolutions; 

    private AudioController _audioBGMController;
    private AudioController _audioSFXController;
    private DialogueController _dialogueController;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _availableResolutions = Screen.resolutions;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _audioBGMController = audioBGMObject.GetComponent<AudioController>();
        _audioSFXController = audioSFXObject.GetComponent<AudioController>();
        _dialogueController = dialogueObject.GetComponent<DialogueController>();

        LoadOptions();
    }

    void SaveOptions()
    {
        PlayerPrefs.SetFloat("TypeSpeed", options.text.typeSpeed);
        PlayerPrefs.SetFloat("AutoSpeed", options.text.autoSpeed);

        PlayerPrefs.SetFloat("BGMVolume", options.audio.bgmVolume);
        PlayerPrefs.SetFloat("SFXVolume", options.audio.sfxVolume);
        PlayerPrefs.SetFloat("VoiceVolume", options.audio.voiceVolume);
        PlayerPrefs.SetFloat("BGMMute", options.audio.bgmMute ? 1 : 0);
        PlayerPrefs.SetFloat("SFXMute", options.audio.sfxMute ? 1 : 0);
        PlayerPrefs.SetFloat("VoiceMute", options.audio.voiceMute ? 1 : 0);

        PlayerPrefs.SetInt("ResolutionIndex", options.resolution.index);
        PlayerPrefs.SetInt("IsFullScreen", options.resolution.isFullScreen ? 1 : 0);

        PlayerPrefs.Save();
    }

    void LoadOptions()
    {
        options.text.typeSpeed = PlayerPrefs.GetFloat("TypeSpeed", options.text.typeSpeed);
        options.text.autoSpeed = PlayerPrefs.GetFloat("AutoSpeed", options.text.autoSpeed);

        options.audio.bgmVolume = PlayerPrefs.GetFloat("BGMVolume", options.audio.bgmVolume);
        options.audio.sfxVolume = PlayerPrefs.GetFloat("SFXVolume", options.audio.sfxVolume);
        options.audio.voiceVolume = PlayerPrefs.GetFloat("VoiceVolume", options.audio.voiceVolume);
        options.audio.bgmMute = PlayerPrefs.GetInt("BGMMute", 0) == 1;
        options.audio.sfxMute = PlayerPrefs.GetInt("SFXMute", 0) == 1;
        options.audio.voiceMute = PlayerPrefs.GetInt("VoiceMute", 0) == 1;

        options.resolution.isFullScreen = PlayerPrefs.GetInt("IsFullScreen", 0) == 1;
        options.resolution.index = PlayerPrefs.GetInt("ResolutionIndex", 0);

        ApplyOptions();
    }

    public void ApplyOptions()
    {
        _audioBGMController.SetOption(options.audio.bgmVolume, options.audio.bgmMute);
        _audioSFXController.SetOption(options.audio.sfxVolume, options.audio.sfxMute);
    }

    public void ApplyResolution()
    {
        if (_availableResolutions.Length == 0) return;

        int index = Mathf.Clamp(options.resolution.index, 0, _availableResolutions.Length - 1);
        Resolution res = _availableResolutions[index];

        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    public void SetVoiceOption(float voiceVolume, bool voiceMute)
    {
        options.audio.voiceVolume = voiceVolume;
        options.audio.voiceMute = voiceMute;
        VoiceOptionChange?.Invoke(options.audio.voiceVolume, options.audio.voiceMute);
    }

    public void SetResolution(int index)
    {
        options.resolution.index = index;
        ApplyResolution();
        SaveOptions();
    }
}
