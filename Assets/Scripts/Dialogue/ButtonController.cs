using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TextMeshProUGUI _text;

    public static ChoiceController choiceController;
    public static SceneController sceneController;

    private ChoiceController _choiceController;
    private SceneController _sceneController;
    private Button _button;

    public ChoiceData choiceData;

    void Start()
    {
        _button = GetComponent<Button>();

        if(ButtonController.sceneController is null)
        {
            GameObject sceneObject = GameObject.Find("Scene");
            sceneController = sceneObject.GetComponent<SceneController>();
        }

        if(ButtonController.choiceController is null)
        {
            GameObject choiceObject = GameObject.Find("Choice");
            choiceController = choiceObject.GetComponent<ChoiceController>();
        }

        _choiceController = choiceController;
        _sceneController = sceneController;

        _button.onClick.AddListener(ClickButton);
    }

    public void ClickButton()
    {
        _sceneController.HandleClickChoiceButton(choiceData);
    }

    public void SetChoiceData(ChoiceData data)
    {
        choiceData = data;
        SetText();
    }

    void SetText()
    {
        _text.text = choiceData.text;
    }

    public IEnumerator FadeIn(float durationTime)
    {
        float proceedTime = 0f;

        while (proceedTime < durationTime)
        {
            proceedTime += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Clamp01(1 - (proceedTime / durationTime));
            yield return null;
        }

        _canvasGroup.alpha = 0f;
    }

    public IEnumerator FadeOut(float durationTime)
    {
        float proceedTime = 0f;

        while (proceedTime < durationTime)
        {
            proceedTime += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Clamp01((proceedTime / durationTime));
            yield return null;
        }

        _canvasGroup.alpha = 1f;
    }

    public void ReturnToPool()
    {
        _choiceController.ReturnButtonObject(gameObject);
    }
}
