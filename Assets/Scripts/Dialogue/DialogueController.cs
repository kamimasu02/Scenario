using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueController : MonoBehaviour
{
    // Type 스피드 조정해서 옵션에서 값을 받아오도록 수정
    public class TextOption
    {
        public float typeSpeed = 0.05f;
        public float autoSpeed = 0.5f;
    }

    [SerializeField] GameObject sceneObject;

    [SerializeField] TextMeshProUGUI dialogueName;
    [SerializeField] TextMeshProUGUI dialogueText;

    // 하단 상수는 옵션에 필요한 값과 그렇지 않은 값으로 분리 필요
    [SerializeField] float DEFAULT_TYPE_SPEED = 0.05f;
    [SerializeField] int DEFAULT_FONT_SIZE = 48;

    public bool isTyping = false;
    public bool isSkipTyping = false;
    public bool isTypingPaused = false;
    public bool hasNextText = false;

    private SceneController _sceneController;

    private Color _fontColor;

    private List<string> TAGS_WITHOUT_CLOSE = new List<string>{ "br" };

    private string _PAUSE_TAG = "pause";

    private string _currentName = "";
    private string _currentText = "";
    private string _scriptName = "";
    private string _scriptText = "";
    private string _tag = "";

    private float _typeSpeed = 0.05f;
    
    private int _id = -1;
    private int _voiceIndex = -1;
    private int _fontSize = 48;

    private Regex _tokenRegex = new Regex(@"<.*?>|.");
    private Regex _tagRegex = new Regex(@"</?(\w+)=?.*?>");
    private Regex _pauseRegex = new Regex(@"(<pause=(\d+(\.\d+)?)>)");
    private Regex _pauseTimeRegex = new Regex(@"<pause=(\d+(\.\d+)?)>");

    private TextOption _option = new TextOption();

    #nullable enable
    private VoiceController? _currentVoiceController = null;
    #nullable disable

    void Start()
    {
        _sceneController = sceneObject.GetComponent<SceneController>();

        Initialize();
    }

    void  Initialize()
    {
        dialogueName.text = "";
        dialogueText.text = "";
    }

    public IEnumerator SetScriptData(DialogueData data)
    {
        if(_currentName != data.name)
        {
            _currentName = data.name;
        }

        _id = data.id;
        _scriptName = data.name;
        _scriptText = data.text.Trim('"');
        _voiceIndex = data.voiceIndex;
        _tag = data.tag;

        _typeSpeed = data.typeSpeed > 0 ? data.typeSpeed : DEFAULT_TYPE_SPEED;
        _fontColor = data.fontColor;
        _fontSize = data.fontSize > 0 ? data.fontSize : DEFAULT_FONT_SIZE;

        if(_currentVoiceController is not null)
        {
            _currentVoiceController.Stop();
            _currentVoiceController = null;
        }

        isTyping = true;
        _currentText = "";
        dialogueName.text = _currentName;
        dialogueText.text = "";
        dialogueText.color = _fontColor;
        dialogueText.fontSize = _fontSize;

        Queue<string> textQueue = new Queue<string>();

        string[] splitedText = _scriptText.Split($"<{_PAUSE_TAG}>");

        for(int i = 0; i < splitedText.Length; i++)
        {
            textQueue.Enqueue(splitedText[i]);
        }

        string prevScript = "";

        if(_voiceIndex > 0)
        {
            GameObject charObject = GameObject.FindGameObjectWithTag(_tag);

            VoiceController voiceController = charObject.GetComponent<VoiceController>();
            voiceController.Play(_voiceIndex);
            _currentVoiceController = voiceController;
        }

        while(textQueue.Count > 0)
        {
            isSkipTyping = false;
            isTypingPaused = false;

            string queuedText = textQueue.Dequeue();

            if(textQueue.Count > 0)
            {
                hasNextText = true;
            }

            Stack<string> tokenStack = new Stack<string>();
            MatchCollection regexMatches = _tokenRegex.Matches(queuedText);

            string currentScript = "";
            string closeTokens = "";

            foreach (Match matchToken in regexMatches)
            {
                if(isSkipTyping)
                {
                    _currentText = prevScript + RemoveAllPauseTags(queuedText);
                    dialogueText.text = _currentText;
                    break;
                }

                string token = matchToken.ToString();

                if(token.StartsWith("<") && token.EndsWith(">"))
                {
                    if(!token.StartsWith("</"))
                    {
                        Match tagMatch = _tagRegex.Match(token);
                        string tokenTag = tagMatch.Groups[1].Value;

                        if(tokenTag == _PAUSE_TAG)
                        {
                            Match pauseMatch = _pauseTimeRegex.Match(token);
                            float pauseTime = float.Parse(pauseMatch.Groups[1].Value);
                            yield return new WaitForSeconds(pauseTime);
                            continue;
                        }
                        else
                        {
                            tokenStack.Push(tokenTag);
                        }
                    }
                    else
                    {
                        Match tagMatch = _tagRegex.Match(token);
                        string tokenTag = tagMatch.Groups[1].Value;
                        tokenStack = RemoveTokenFromStack(tokenTag, tokenStack);
                    }
                }

                closeTokens = tokenStack.Count > 0 ? CreateCloseTokens(tokenStack) : "";
                currentScript += token;
                _currentText = prevScript + currentScript + closeTokens;
                dialogueText.text = _currentText;
                yield return new WaitForSeconds(_typeSpeed);
            }

            prevScript = _currentText;
            isTypingPaused = true;
            yield return new WaitUntil(() => textQueue.Count == 0 || !hasNextText);
        }

        isTyping = false;
    }

    public void SkipTyping()
    {
        isSkipTyping = true;
    }

    public void PrintNextText()
    {
        hasNextText = false;
    }

    string CreateCloseTokens(Stack<string> tokens)
    {
        Stack<string> copiedTokens = new Stack<string>(tokens);
        string result = "";

        while(copiedTokens.Count > 0)
        {
            string token = copiedTokens.Pop();

            if(TAGS_WITHOUT_CLOSE.Contains(token))
            {
                continue;
            }

            result += "</" + token + ">";
        }

        return result;
    }

    string RemoveAllPauseTags(string text)
    {
        return _pauseRegex.Replace(text, "");
    }

    Stack<string> RemoveTokenFromStack(string removeToken, Stack<string> tokens)
    {
        bool isTokenExisted = false;

        Queue<string> postTokens = new Queue<string>();
        Stack<string> copiedTokens = new Stack<string>(tokens);

        while(copiedTokens.Count > 0)
        {
            string token = copiedTokens.Pop();

            if(token == removeToken)
            {
                isTokenExisted = true;
                break;
            }
            else
            {
                postTokens.Enqueue(token);
            }
        }

        if(isTokenExisted)
        {
            while(postTokens.Count > 0)
            {
                string token = postTokens.Dequeue();
                copiedTokens.Push(token);
            }

            return copiedTokens;
        }
        else
        {
            Debug.LogError($"{removeToken}이 존재하지 않습니다.");
            return tokens;
        }
    }
}
