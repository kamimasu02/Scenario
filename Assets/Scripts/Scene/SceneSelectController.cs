using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelectController : MonoBehaviour
{
    private LoadingSceneController _loadingSceneController;

    void Start()
    {
      GameObject sceneObject = GameObject.Find("Scene");
      
      if(sceneObject is not null)
      {
        Destroy(sceneObject);
      }
    }

    void Update()
    {
      if(Input.GetKeyDown(KeyCode.Escape))
      {
          Application.Quit();
      }      
    }

    public void LoadScene()
    {
      SceneManager.LoadScene("LoadingScene", LoadSceneMode.Single);
    }
}
