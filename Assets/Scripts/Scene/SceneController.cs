using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    [SerializeField] private CSVLoader _csvLoader;
    [SerializeField] private CSVReader _csvReader;


    [SerializeField] private AudioController _audioBGMController;
    [SerializeField] private AudioController _audioSFXController;

    [SerializeField] private ChoiceController _choiceController;
    [SerializeField] private DialogueController _dialogueController;

    [SerializeField] private ImageController _itemImageController;
    [SerializeField] private ImageController _backgroundImageController;
    [SerializeField] private ImageController _foregroundImageController;
    [SerializeField] private ImageController _sceneImageController;

    private SceneSO _currentScene;

    private bool _isAutoPass = false;
    private bool _isChoiceScript = false;
    private bool _isSceneOver = false;
    private bool _blockScript = false;
    private bool _blockUserInteraction = false;

    private Queue<ScriptData> _currentScriptData;

    private AudioData[] _audioBGMData;
    private AudioData[] _audioSFXData;
    private CharacterData[] _charData;
    private ChoiceData[] _choiceData;
    private DialogueData[] _dialogueData;
    private IconData[] _iconData;
    private ImageData[] _backgroundImageData;
    private ImageData[] _foregroundImageData;
    private ImageData[] _itemImageData;
    private ImageData[] _sceneImageData;
    private ScriptData[] _scriptData;
    private SpriteData[] _spriteData;

    private Dictionary<GameObject, SpriteController> _spriteControllerDict = new Dictionary<GameObject, SpriteController>();
    
    [Header("ETC")]
    public CoroutineManager coroutineManager;

    void Start()
    {
        coroutineManager = GetComponent<CoroutineManager>();

        _currentScene = SceneDataManager.Instance.GetCurrentScene();

        LoadSceneData();
    }

    void Update()
    {
        CheckSceneIsOver();
        ProcessScript();
    }

    void CheckSceneIsOver()
    {
        if(_isSceneOver)
        {
            QuitScene();
        }
    }

    void ProcessScript()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if(_isAutoPass)
        {
            if(_blockScript) return;

            if(_dialogueController.isTyping && _dialogueController.isTypingPaused)
            {
                _dialogueController.PrintNextText();
                return;
            }
            
            ReadScript();
            return;
        }
        else
        {
            if(_isChoiceScript) return;
            
            if(!Input.GetKeyDown(KeyCode.Mouse0) && !Input.GetKeyDown(KeyCode.Space) && !Input.GetKeyDown(KeyCode.Return)) return;
            
            if(_blockScript)
            {
                if(_blockUserInteraction || !_dialogueController.isTyping) return;
        
                if(_dialogueController.isTypingPaused)
                {
                    _dialogueController.PrintNextText();
                }
                else
                {
                    _dialogueController.SkipTyping();
                }
                return;
            }

            ReadScript();
        }
    }

    void ReadScript()
    {
        if(_currentScriptData.Count > 0)
        {
            ScriptData script = _currentScriptData.Dequeue();

            SetBlockScript(true);

            ProcessSpriteData(script.spriteId);
            ProcessAudioData(script.audioBGMId, script.audioSFXId);
            ProcessImageData(script.backgroundId, script.foregroundId, script.itemId, script.sceneId);
            ProcessDialogueData(script.dialogueId, script.choiceId);

            SetDialogueOption(script.blockInteraction, script.autoPass);

            StartCoroutine(coroutineManager.CheckAllCoroutinesCompleted(InitializeAfterScriptProcess));
        }
        else
        {
            _isSceneOver = true;
        }
    }

    void InitializeAllCharData()
    {
        foreach(CharacterData charDatum in _charData)
        {
            GameObject charObject = GameObject.FindGameObjectWithTag(charDatum.tag);
            SpriteController charSpriteController = charObject.GetComponent<SpriteController>();

            _spriteControllerDict[charObject] = charSpriteController;
            charSpriteController.Initialize(charDatum);
        }
    }

    void LoadAudioData()
    {
        _audioBGMData = _csvReader.LoadData<AudioData>(_csvLoader, $"{_currentScene.path}/Audio/bgm");
        _audioSFXData = _csvReader.LoadData<AudioData>(_csvLoader, $"{_currentScene.path}/Audio/sfx");
    }

    void LoadImageData()
    {
        _charData = _csvReader.LoadData<CharacterData>(_csvLoader, $"{_currentScene.path}/Image/character");
        _foregroundImageData = _csvReader.LoadData<ImageData>(_csvLoader, $"{_currentScene.path}/Image/foreground");
        _backgroundImageData = _csvReader.LoadData<ImageData>(_csvLoader, $"{_currentScene.path}/Image/background");
        _iconData = _csvReader.LoadData<IconData>(_csvLoader, $"{_currentScene.path}/Image/icon");
        _itemImageData = _csvReader.LoadData<ImageData>(_csvLoader, $"{_currentScene.path}/Image/item");
        _sceneImageData = _csvReader.LoadData<ImageData>(_csvLoader, $"{_currentScene.path}/Image/scene");
        _spriteData = _csvReader.LoadData<SpriteData>(_csvLoader, $"{_currentScene.path}/Image/sprite");
    }

    void LoadDialogueData()
    {
        _choiceData = _csvReader.LoadData<ChoiceData>(_csvLoader, $"{_currentScene.path}/Dialogue/choice");
        _dialogueData = _csvReader.LoadData<DialogueData>(_csvLoader, $"{_currentScene.path}/Dialogue/dialogue");
    }

    void ProcessSpriteData(int spriteId)
    {
        if(spriteId >= 0)
        {
            SpriteData[] filteredSpriteData = _spriteData.Where(datum => datum.groupId == spriteId).ToArray();
            
            foreach(SpriteData spriteDatum in filteredSpriteData)
            {
                GameObject charObject = GameObject.FindGameObjectWithTag(spriteDatum.tag);
                SpriteController spriteController = _spriteControllerDict[charObject];

                if(spriteDatum.iconId >= 0)
                {
                    IconData iconDatum = _iconData.Single(datum => datum.id == spriteDatum.iconId);
                    coroutineManager.StartCoroutineProcess(spriteController.SetSpriteData(spriteDatum, iconDatum));
                }
                else
                {
                    coroutineManager.StartCoroutineProcess(spriteController.SetSpriteData(spriteDatum));
                }
            }
        }
    }

    void ProcessAudioData(int bgmId, int sfxId)
    {
        if(bgmId >= 0)
        {
            AudioData audioDatum = _audioBGMData.FirstOrDefault(datum => datum.id == bgmId);
            _audioBGMController.SetData(audioDatum);
        }

        if(sfxId >= 0)
        {
            AudioData audioDatum = _audioSFXData.FirstOrDefault(datum => datum.id == sfxId);
            _audioSFXController.SetData(audioDatum);
        }
    }

    void ProcessImageData(int backgroundId, int foregroundId, int itemId, int sceneId)
    {
        if(backgroundId >= 0)
        {
            ImageData imageDatum = _backgroundImageData.FirstOrDefault(datum => datum.id == backgroundId);

            if(imageDatum is not null)
            {
                coroutineManager.StartCoroutineProcess(_backgroundImageController.SetImageData(imageDatum));
            }
        }

        if(foregroundId >= 0)
        {
            ImageData imageDatum = _foregroundImageData.FirstOrDefault(datum => datum.id == foregroundId);

            if(imageDatum is not null)
            {
                coroutineManager.StartCoroutineProcess(_foregroundImageController.SetImageData(imageDatum));   
            }
        }

        if(itemId >= 0)
        {
            ImageData imageDatum = _itemImageData.FirstOrDefault(datum => datum.id == itemId);

            if(imageDatum is not null)
            {
                coroutineManager.StartCoroutineProcess(_itemImageController.SetImageData(imageDatum));   
            }
        }

        if(sceneId >= 0)
        {
            ImageData imageDatum = _sceneImageData.FirstOrDefault(datum => datum.id == sceneId);

            if(imageDatum is not null)
            {
                coroutineManager.StartCoroutineProcess(_sceneImageController.SetImageData(imageDatum));   
            }
        }
    }

    void ProcessDialogueData(int dialogueId, int choiceId)
    {
        if(dialogueId >= 0)
        {
            DialogueData dialogueDatum = _dialogueData.FirstOrDefault(datum => datum.id == dialogueId);
            
            if(dialogueDatum is not null)
            {
                coroutineManager.StartCoroutineProcess(_dialogueController.SetDialogueData(dialogueDatum));
            }
        }

        if(choiceId > 0)
        {
            SetChoiceScript(true);
            ChoiceData[] choiceDataGroupById = _choiceData.Where(datum => datum.groupId == choiceId).ToArray();
            coroutineManager.StartCoroutineProcess(_choiceController.SetChoiceData(choiceDataGroupById));
        }
    }

    void SetDialogueOption(bool blockInteraction, bool autoPass)
    {
        if(autoPass)
        {
            SetAutoPass(true);
        }
        else
        {
            SetAutoPass(false);
        }

        if(blockInteraction)
        {
            SetBlockUserInteraction(true);
        }
    }

    void ProcessChoiceScript()
    {
        SetChoiceScript(false);
        ReadScript();
    }

    void QuitScene()
    {
        SceneManager.LoadScene("SelectScene", LoadSceneMode.Single);
    }

    public void HandleClickChoiceButton(ChoiceData data)
    {
        _choiceController.DisableButtons();

        if(data.scriptFilePathWithName.Length > 0)
        {
            ScriptData[] nextScriptData = _csvReader.LoadData<ScriptData>(_csvLoader, $"{_currentScene.path}/Script/{data.scriptFilePathWithName}");
            _currentScriptData = new Queue<ScriptData>(nextScriptData);
        }

        coroutineManager.StartCoroutineProcess(_choiceController.RemoveButtons(ProcessChoiceScript));
    }

    public void LoadSceneData()
    {
        ScriptData[] csvScriptData = _csvReader.LoadData<ScriptData>(_csvLoader, $"{_currentScene.path}/script");

        _scriptData = csvScriptData;
        _currentScriptData = new Queue<ScriptData>(csvScriptData);

        LoadAudioData();
        LoadImageData();
        LoadDialogueData();

        InitializeAllCharData();

        ReadScript();
    }

    public void SetAutoPass(bool boolValue)
    {
        _isAutoPass = boolValue;
    }

    public void SetChoiceScript(bool boolValue)
    {
        _isChoiceScript = boolValue;
    }

    public void SetBlockScript(bool boolValue)
    {
        _blockScript = boolValue;
    }

    public void SetBlockUserInteraction(bool boolValue)
    {
        _blockUserInteraction = boolValue;
    }

    public void InitializeAfterScriptProcess()
    {
        SetBlockScript(false);
        SetBlockUserInteraction(false);
    }
}
