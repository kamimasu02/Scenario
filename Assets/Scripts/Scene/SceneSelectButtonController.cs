using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneSelectButtonController : MonoBehaviour
{
    [SerializeField] SceneSelectController _sceneSelectController;
    [SerializeField] SceneSO _scene;

    public void OnButtonClick()
    {
        SceneDataManager.Instance.SetCurrentScene(_scene);
        _sceneSelectController.LoadScene();
    }
}
