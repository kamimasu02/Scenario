using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingSceneController : MonoBehaviour
{
    [Header("Loading")]
    [SerializeField] private Slider _loadingProgressBar;
    [SerializeField] private Image _loadingImage;
    [SerializeField] private TextMeshProUGUI _loadingText;
    [SerializeField] private Sprite[] _loadingImages;
    [SerializeField] private RectTransform _iconRectTransform;

    private const float _MIN_LOADING_TIME = 2.0f;
    private const float _ICON_ROTATION_VALUE = 0.5f;
    private const float _ICON_ROTATION_SPEED = 0.5f;
    private const float _MAX_DEGREE = 360;


    void Start()
    {
        if(_loadingImages.Length > 0)
        {
            int imageIndex = Mathf.FloorToInt(Random.Range(0, _loadingImages.Length));
            _loadingImage.sprite = _loadingImages[imageIndex];
        }

        GameObject selectSceneObject = GameObject.Find("Select Scene");

        if(selectSceneObject is not null)
        {
            Destroy(selectSceneObject);
        }

        SceneSO scene = SceneDataManager.Instance.GetCurrentScene();

        StartCoroutine(RotateIcon());
        StartCoroutine(LoadSceneAsync(scene.sceneName));
    }

    void LateUpdate()
    {
        float progress = LoadingDataManager.Instance.GetProgress();

        _loadingProgressBar.value = progress;
        _loadingText.text = progress >= 0.9f ? "100%" : (progress >= 0.1f ? $"{Mathf.FloorToInt(progress * 100f).ToString().PadLeft(4, ' ')}%" : $"{Mathf.FloorToInt(progress * 100f).ToString().PadLeft(5, ' ')}%");
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
