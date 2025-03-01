using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;

    private SceneController _sceneController;

    public ChoiceData choiceData;

    void Start()
    {
        Button button = GetComponent<Button>();

        GameObject sceneObject = GameObject.Find("Scene");
        _sceneController = sceneObject.GetComponent<SceneController>();

        button.onClick.AddListener(ClickButton);
    }

    public void ClickButton()
    {
        _sceneController.HandleClickChoiceButton(choiceData);
    }

    public void SetChoiceData(ChoiceData data)
    {
        choiceData = data;
    }

    public IEnumerator FadeIn(float durationTime)
    {
        float proceedTime = 0f;

        while (proceedTime < durationTime)
        {
            proceedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1 - (proceedTime / durationTime));
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }

    public IEnumerator FadeOut(float durationTime)
    {
        float proceedTime = 0f;

        while (proceedTime < durationTime)
        {
            proceedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01((proceedTime / durationTime));
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }
}
