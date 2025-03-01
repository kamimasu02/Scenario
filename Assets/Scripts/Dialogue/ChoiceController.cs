using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoiceController : MonoBehaviour
{
    [SerializeField] GameObject canvasObjet;
    [SerializeField] GameObject choicePrefab;
    [SerializeField] GameObject sceneObject;

    [SerializeField] float FADE_TIME = 0.3f;

    private float DESTROY_WAITING_TIME = 0.5f;

    private Color DEFAULT_COLOR = new Color(1.0f, 1.0f, 1.0f);

    private SceneController _sceneController;
    private List<GameObject> buttonObjects = new List<GameObject>();

    void Start()
    {
        _sceneController = sceneObject.GetComponent<SceneController>();
    }

    public IEnumerator SetChoiceData(ChoiceData[] data)
    {
        for(int index = 0; index < data.Length; index++)
        {
            ChoiceData choiceDatum = data[index];
            GameObject choiceButton = Instantiate(choicePrefab, canvasObjet.transform);

            Image choiceImage = choiceButton.GetComponent<Image>();

            choiceImage.color = DEFAULT_COLOR;

            choiceButton.transform.localScale = Vector3.one;
            choiceButton.transform.localPosition = Vector3.zero;

            Transform choiceTextTransform = choiceButton.transform.Find("ChoiceText");

            if(choiceTextTransform is not null)
            {
                GameObject choiceText = choiceTextTransform.gameObject;
                TextMeshProUGUI choiceTMP = choiceText.GetComponent<TextMeshProUGUI>();
            
                choiceTMP.text = choiceDatum.text;

                buttonObjects.Add(choiceButton);;
            }
        }

        if(buttonObjects.Count > 0)
        {
            for(int index = 0; index < data.Length; index++)
            {
                GameObject choiceButtonObject = buttonObjects[index];

                ButtonController choiceButtonController = choiceButtonObject.GetComponent<ButtonController>();
                choiceButtonController.SetChoiceData(data[index]);

                _sceneController.coroutineManager.StartCoroutineProcess(choiceButtonController.FadeOut(FADE_TIME));
            }
        }

        yield return null;
    }

    public IEnumerator RemoveButtons(Action callbackAction)
    {
        if(buttonObjects.Count > 0)
        {
            foreach(GameObject choiceButton in buttonObjects)
            {
                ButtonController choiceButtonController = choiceButton.GetComponent<ButtonController>();

                _sceneController.coroutineManager.StartCoroutineProcess(choiceButtonController.FadeIn(FADE_TIME));
            }
        }

        yield return new WaitForSeconds(FADE_TIME + DESTROY_WAITING_TIME);

        foreach(GameObject choiceButton in buttonObjects)
        {
            Destroy(choiceButton);
        }

        buttonObjects.Clear();

        callbackAction();

        yield return null;
    }
}
