using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneSelectButtonController : MonoBehaviour
{
    [SerializeField] GameObject sceneSelectObject;
    [SerializeField] SceneSO scene;

    private SceneSelectController _sceneSelectController;

    void Start()
    {
        _sceneSelectController = sceneSelectObject.GetComponent<SceneSelectController>();
    }

    public void OnButtonClick()
    {
        SceneDataManager.Instance.SetCurrentScene(scene);
        _sceneSelectController.LoadScene();
    }
}
