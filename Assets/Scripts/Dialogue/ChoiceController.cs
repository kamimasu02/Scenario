using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoiceController : MonoBehaviour
{
    [SerializeField] private SceneController _sceneController;
    [SerializeField] private GameObject _canvasObject;

    [SerializeField] private GameObject choicePrefab;

    [SerializeField] private const float _FADE_TIME = 0.3f;

    [SerializeField] private const int _INITIAL_BUTTON_POOL_SIZE = 3;

    private List<GameObject> _buttonObjects = new List<GameObject>();
    private Queue<GameObject> _buttonPool = new Queue<GameObject>();
    private Dictionary<GameObject, Button> _buttonDict = new Dictionary<GameObject, Button>();
    private Dictionary<GameObject, ButtonController> _buttonControllerDict = new Dictionary<GameObject, ButtonController>();

    void Start()
    {
        for (int i = 0; i < _INITIAL_BUTTON_POOL_SIZE; i++)
        {
            GameObject choiceButtonObject = Instantiate(choicePrefab, _canvasObject.transform);
            InitializeButton(choiceButtonObject);
            _buttonPool.Enqueue(choiceButtonObject);
        }
    }

    void InitializeButton(GameObject choiceButtonObject)
    {
        choiceButtonObject.SetActive(false);
        
        _buttonDict[choiceButtonObject] = choiceButtonObject.GetComponent<Button>();
        _buttonControllerDict[choiceButtonObject] = choiceButtonObject.GetComponent<ButtonController>();

        Transform choiceTextTransform = choiceButtonObject.transform.Find("ChoiceText");
    }

    public IEnumerator SetChoiceData(ChoiceData[] data)
    {
        for(int index = 0; index < data.Length; index++)
        {
            ChoiceData choiceDatum = data[index];
            GameObject choiceButtonObject = GetButtonObject();

            choiceButtonObject.transform.localScale = Vector3.one;
            choiceButtonObject.transform.localPosition = Vector3.zero;

            ButtonController choiceButtonController = _buttonControllerDict[choiceButtonObject];
            choiceButtonController.SetChoiceData(data[index]);

            _buttonObjects.Add(choiceButtonObject);
        }

        foreach(GameObject choiceButtonObject in _buttonObjects)
        {
            ButtonController choiceButtonController = _buttonControllerDict[choiceButtonObject];
            _sceneController.coroutineManager.StartCoroutineProcess(choiceButtonController.FadeOut(_FADE_TIME));
        }

        yield return new WaitForSeconds(_FADE_TIME);

        EnableButtons();
    }

    public void EnableButtons()
    {
        foreach(GameObject choiceButtonObject in _buttonObjects)
        {
            Button button = _buttonDict[choiceButtonObject];
            button.interactable = true;
        }
    }

    public void DisableButtons()
    {
        foreach(GameObject choiceButtonObject in _buttonObjects)
        {
            Button button = _buttonDict[choiceButtonObject];
            button.interactable = false;
        }
    }

    public IEnumerator RemoveButtons(Action callbackAction)
    {
        
        foreach(GameObject choiceButtonObject in _buttonObjects)
        {
            ButtonController choiceButtonController = _buttonControllerDict[choiceButtonObject];
            _sceneController.coroutineManager.StartCoroutineProcess(choiceButtonController.FadeIn(_FADE_TIME));
        }

        yield return new WaitForSeconds(_FADE_TIME);

        foreach(GameObject choiceButtonObject in _buttonObjects)
        {
            ButtonController choiceButtonController = _buttonControllerDict[choiceButtonObject];
            choiceButtonController.ReturnToPool();
        }

        _buttonObjects.Clear();

        callbackAction();

        yield return null;
    }

    GameObject GetButtonObject()
    {
        if(_buttonPool.Count > 0)
        {
            GameObject buttonObject = _buttonPool.Dequeue();
            buttonObject.SetActive(true);
            return buttonObject;
        }
        else
        {
            GameObject buttonObject = Instantiate(choicePrefab, _canvasObject.transform);
            InitializeButton(buttonObject);
            return buttonObject;
        }
    }

    public void ReturnButtonObject(GameObject buttonObject)
    {
        buttonObject.SetActive(false);
        _buttonPool.Enqueue(buttonObject);
    }
}
