using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingDataManager : MonoBehaviour
{
    public static LoadingDataManager Instance;

    private float _progress = 0f;

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

    public void InitializeProgress()
    {
        _progress = 0f;
    }

    public void SetProgress(float progress)
    {
        _progress = progress;
    }

    public float GetProgress()
    {
        return _progress;
    }
}
