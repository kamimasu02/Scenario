using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneDataManager : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] SceneGroupSO[] sceneGroups;

    public static SceneDataManager Instance;

    private SceneSO _currentScene;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCurrentScene(SceneSO scene)
    {
        _currentScene = scene;
    }

    // 추후에 메인, 단편 스토리 구분 될때 사용
    // public void SetCurrentScene(string sceneCategory, int sceneId)
    // {
    //     SceneGroupSO sceneGroup = sceneGroups.Single(sceneGroup => sceneGroup.category == sceneCategory);
    //     _currentScene = sceneGroup.scenes.Single(scene => scene.id == sceneId);
    // }

    public SceneSO GetCurrentScene()
    {
        return _currentScene;
    }
}
