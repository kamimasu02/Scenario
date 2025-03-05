using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingSceneController : MonoBehaviour
{
    [Header("Loading")]
    [SerializeField] Slider loadingProgressBar;
    [SerializeField] Image loadingImage;
    [SerializeField] TextMeshProUGUI loadingText;
    [SerializeField] GameObject iconObject;
    [SerializeField] Sprite[] loadingImages;

    private float _MIN_LOADING_TIME = 2.0f;
    private float _ICON_ROTATION_VALUE = 0.5f;
    private float _ICON_ROTATION_SPEED = 0.5f;
    private float _MAX_DEGREE = 360;

    private RectTransform _iconRectTransform;

    void Start()
    {
        int imageIndex = Mathf.FloorToInt(Random.Range(0, loadingImages.Length));
        loadingImage.sprite = loadingImages[imageIndex];

        GameObject selectSceneObject = GameObject.Find("Select Scene");

        if(selectSceneObject is not null)
        {
            Destroy(selectSceneObject);
        }

        _iconRectTransform = iconObject.GetComponent<RectTransform>();

        SceneSO scene = SceneDataManager.Instance.GetCurrentScene();

        StartCoroutine(RotateIcon());
        StartCoroutine(LoadSceneAsync(scene.sceneName));
    }

    void LateUpdate()
    {
        float progress = LoadingDataManager.Instance.GetProgress();

        loadingProgressBar.value = progress;
        loadingText.text = progress >= 0.9f ? "100%" : (progress >= 0.1f ? $"{Mathf.FloorToInt(progress * 100f).ToString().PadLeft(4, ' ')}%" : $"{Mathf.FloorToInt(progress * 100f).ToString().PadLeft(5, ' ')}%");
    }

    IEnumerator RotateIcon()
    {
        while(LoadingDataManager.Instance.GetProgress() <= 1f)
        {
            _iconRectTransform.eulerAngles = new Vector3(0, 0, (_iconRectTransform.eulerAngles.z - _ICON_ROTATION_VALUE) % _MAX_DEGREE);
            yield return new WaitForSeconds(_ICON_ROTATION_SPEED * Time.deltaTime);
        }
    }

    public IEnumerator LoadSceneAsync(string SceneName)
    {
      LoadingDataManager.Instance.InitializeProgress();

      AsyncOperation loadSceneOperation = SceneManager.LoadSceneAsync(SceneName);
      loadSceneOperation.allowSceneActivation = false;

      float proceedTime = 0f;

      while(!loadSceneOperation.isDone)
      {
          float progress = Mathf.Clamp01(loadSceneOperation.progress / 0.9f);
          LoadingDataManager.Instance.SetProgress(progress);

          proceedTime += Time.deltaTime;

          if(progress >= 1f && proceedTime >= _MIN_LOADING_TIME)
          {
            break;
          }

          yield return null;
      }
      
      yield return new WaitForSeconds(Mathf.Max(0, _MIN_LOADING_TIME - proceedTime));

      loadSceneOperation.allowSceneActivation = true;
    }
}
