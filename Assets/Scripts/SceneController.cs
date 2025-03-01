using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] SceneGroupSO[] sceneGroups;
    [Header("CSVs")]
    [SerializeField] GameObject csvObject;

    [Header("Dialogues")]
    [SerializeField] GameObject choiceObject;
    [SerializeField] GameObject dialogueObject;

    [Header("Images")]
    [SerializeField] GameObject itemImageObject;
    [SerializeField] GameObject backgroundImageObject;
    [SerializeField] GameObject foregroundImageObject;
    [SerializeField] GameObject sceneImageObject;
    

    [Header("Audios")]
    [SerializeField] GameObject audioBGMObject;
    [SerializeField] GameObject audioSFXObject;


    private SceneSO _currentScene;
    private CSVLoader _csvLoader;
    private AudioCSVReader _audioCSVReader;
    private CharacterCSVReader _charCSVReader;
    private ChoiceCSVReader _choiceCSVReader;
    private DialogueCSVReader _dialogueCSVReader;
    private IconCSVReader _iconCSVReader;
    private ImageCSVReader _imageCSVReader;
    private ScriptCSVReader _scriptCSVReader;
    private SpriteCSVReader _spriteCSVReader;

    private AudioController _audioBGMController;
    private AudioController _audioSFXController;

    private ChoiceController _choiceController;
    private DialogueController _dialogueController;

    private ImageController _itemImageController;
    private ImageController _backgroundImageController;
    private ImageController _foregroundImageController;
    private ImageController _sceneImageController;

    private bool _isAutoPass = false;
    private bool _isChoiceScript = false;
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
    private ImageData[] _sceneImageData;
    private ImageData[] _itemImageData;
    private ScriptData[] _scriptData;
    private SpriteData[] _spriteData;
    
    [Header("ETC")]
    public CoroutineManager coroutineManager;

    void Start()
    {
        coroutineManager = GetComponent<CoroutineManager>();

        _csvLoader = csvObject.GetComponent<CSVLoader>();

        _audioCSVReader = csvObject.GetComponent<AudioCSVReader>();
        _charCSVReader = csvObject.GetComponent<CharacterCSVReader>();
        _choiceCSVReader = csvObject.GetComponent<ChoiceCSVReader>();
        _dialogueCSVReader = csvObject.GetComponent<DialogueCSVReader>();
        _iconCSVReader = csvObject.GetComponent<IconCSVReader>();
        _imageCSVReader = csvObject.GetComponent<ImageCSVReader>();
        _scriptCSVReader = csvObject.GetComponent<ScriptCSVReader>();
        _spriteCSVReader = csvObject.GetComponent<SpriteCSVReader>();

        _audioBGMController = audioBGMObject.GetComponent<AudioController>();
        _audioSFXController = audioSFXObject.GetComponent<AudioController>();

        _choiceController = choiceObject.GetComponent<ChoiceController>();
        _dialogueController = dialogueObject.GetComponent<DialogueController>();

        _itemImageController = itemImageObject.GetComponent<ImageController>();
        _backgroundImageController = backgroundImageObject.GetComponent<ImageController>();
        _foregroundImageController = foregroundImageObject.GetComponent<ImageController>();
        _sceneImageController = sceneImageObject.GetComponent<ImageController>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if(_isAutoPass)
        {
            if(!_blockScript)
            {
                if(_dialogueController.isTyping && _dialogueController.isTypingPaused)
                {
                    _dialogueController.PrintNextText();
                }
                else
                {
                    ReadScript();
                }
            }
        }
        else
        {
            if(!_isChoiceScript)
            {
                if(Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
                {
                    if(!_blockScript)
                    {
                        ReadScript();
                    }
                    else
                    {
                        if(!_blockUserInteraction)
                        {
                            if(_dialogueController.isTyping)
                            {
                                if(_dialogueController.isTypingPaused)
                                {
                                    _dialogueController.PrintNextText();
                                }
                                else
                                {
                                    _dialogueController.SkipTyping();
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void ReadScript()
    {
        if(_currentScriptData.Count > 0)
        {
            ScriptData script = _currentScriptData.Dequeue();

            SetBlockScript(true);
            
            int id = script.id;
            int choiceId = script.choiceId;
            int dialogueId = script.dialogueId;
            int spriteId = script.spriteId;
            int itemId = script.itemId;
            int backgroundId = script.backgroundId;
            int foregroundId = script.foregroundId;
            int sceneId = script.sceneId;
            int audioBGMId = script.audioBGMId;
            int audioSFXId = script.audioSFXId;
            bool autoPass = script.autoPass;
            bool blockInteraction = script.blockInteraction;

            if(spriteId >= 0)
            {
                SpriteData[] filteredSpriteData = _spriteData.Where(datum => datum.groupId == spriteId).ToArray();
                
                foreach(SpriteData spriteDatum in filteredSpriteData)
                {
                    GameObject charObject = GameObject.FindGameObjectWithTag(spriteDatum.tag);
                    SpriteController spriteController = charObject.GetComponent<SpriteController>();

                    if(spriteDatum.iconId >= 0)
                    {
                        IconData iconDatum = _iconData.Where(datum => datum.id == spriteDatum.iconId).ToArray()[0];
                        coroutineManager.StartCoroutineProcess(spriteController.SetSpriteData(spriteDatum, iconDatum));
                    }
                    else
                    {
                        coroutineManager.StartCoroutineProcess(spriteController.SetSpriteData(spriteDatum));
                    }
                }
            }

            if(audioBGMId > 0)
            {
                AudioData audioDatum = _audioBGMData.Where(datum => datum.id == audioBGMId).ToArray()[0];
                
                if(audioDatum.index > 0)
                {
                    _audioBGMController.Play(audioDatum);
                }
                else
                {
                    _audioBGMController.Stop(audioDatum);
                }
            }
            else if(audioBGMId == 0)
            {   
                _audioBGMController.Stop();
            }

            if(audioSFXId > 0)
            {
                AudioData audioDatum = _audioSFXData.Where(datum => datum.id == audioSFXId).ToArray()[0];

                if(audioDatum.index > 0)
                {
                    _audioSFXController.Play(audioDatum);
                }
                else
                {
                    _audioSFXController.Stop(audioDatum);
                }
            }
            else if(audioSFXId == 0)
            {
                _audioSFXController.Stop();
            }

            if(itemId >= 0)
            {
                ImageData imageDatum = _itemImageData.Where(datum => datum.id == itemId).ToArray()[0];
                coroutineManager.StartCoroutineProcess(_itemImageController.SetImageData(imageDatum));
            }

            if(backgroundId >= 0)
            {
                ImageData imageDatum = _backgroundImageData.Where(datum => datum.id == backgroundId).ToArray()[0];
                coroutineManager.StartCoroutineProcess(_backgroundImageController.SetImageData(imageDatum));
            }

            if(foregroundId >= 0)
            {
                ImageData imageDatum = _foregroundImageData.Where(datum => datum.id == foregroundId).ToArray()[0];
                coroutineManager.StartCoroutineProcess(_foregroundImageController.SetImageData(imageDatum));
            }

            if(sceneId >= 0)
            {
                ImageData imageDatum = _sceneImageData.Where(datum => datum.id == sceneId).ToArray()[0];
                coroutineManager.StartCoroutineProcess(_sceneImageController.SetImageData(imageDatum));
            }

            if(dialogueId >= 0)
            {
                DialogueData dialogueDatum = _dialogueData.Where(datum => datum.id == dialogueId).ToArray()[0];
                coroutineManager.StartCoroutineProcess(_dialogueController.SetScriptData(dialogueDatum));

                if(choiceId > 0)
                {
                    SetChoiceScript(true);
                    ChoiceData[] choiceDataGroupById = _choiceData.Where(datum => datum.groupId == choiceId).ToArray();
                    coroutineManager.StartCoroutineProcess(_choiceController.SetChoiceData(choiceDataGroupById));
                }
            }

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
            

            StartCoroutine(coroutineManager.CheckAllCoroutinesCompleted(InitializeAfterScriptProcess));
        }
    }

    void InitializeAllCharData()
    {
        foreach(CharacterData charDatum in _charData)
        {
            GameObject charObject = GameObject.FindGameObjectWithTag(charDatum.tag);
            SpriteController charSpriteController = charObject.GetComponent<SpriteController>();
            charSpriteController.Initialize(charDatum);
        }
    }

    void LoadAudioData()
    {
        _audioBGMData = _audioCSVReader.LoadData(_csvLoader, $"{_currentScene.path}/Audio/bgm");
        _audioSFXData = _audioCSVReader.LoadData(_csvLoader, $"{_currentScene.path}/Audio/sfx");
    }

    void LoadImageData()
    {
        _charData = _charCSVReader.LoadData(_csvLoader, $"{_currentScene.path}/Image/character");
        _foregroundImageData = _imageCSVReader.LoadData(_csvLoader, $"{_currentScene.path}/Image/foreground");
        _backgroundImageData = _imageCSVReader.LoadData(_csvLoader, $"{_currentScene.path}/Image/background");
        _iconData = _iconCSVReader.LoadData(_csvLoader, $"{_currentScene.path}/Image/icon");
        _sceneImageData = _imageCSVReader.LoadData(_csvLoader, $"{_currentScene.path}/Image/scene");
        _spriteData = _spriteCSVReader.LoadData(_csvLoader, $"{_currentScene.path}/Image/sprite");
    }

    void LoadDialogueData()
    {
        _choiceData = _choiceCSVReader.LoadData(_csvLoader, $"{_currentScene.path}/Dialogue/choice");
        _dialogueData = _dialogueCSVReader.LoadData(_csvLoader, $"{_currentScene.path}/Dialogue/dialogue");
    }

    void ProcessChoiceScript()
    {
        SetChoiceScript(false);
        ReadScript();
    }

    public void HandleClickChoiceButton(ChoiceData data)
    {
        _currentScriptData = new Queue<ScriptData>(_scriptData[data.startIndex..(data.endIndex + 1)]);
        coroutineManager.StartCoroutineProcess(_choiceController.RemoveButtons(ProcessChoiceScript));
    }

    public void LoadSceneData(string sceneCategory, int sceneId)
    {
        SceneGroupSO sceneGroup = sceneGroups.Where(sceneGroup => sceneGroup.category == sceneCategory).ToArray()[0];
        _currentScene = sceneGroup.scenes.Where(scene => scene.id == sceneId).ToArray()[0];

        ScriptData[] csvScriptData = _scriptCSVReader.LoadData(_csvLoader, $"{_currentScene.path}/script");

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
