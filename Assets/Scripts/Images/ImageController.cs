using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageController : MonoBehaviour
{
    [Header("Transform")]
    [SerializeField] RectTransform rt;

    [Header("Image")]
    [SerializeField] GameObject scene;
    [SerializeField] GameObject prevImgObject;
    [SerializeField] Image img;
    [SerializeField] Sprite[] sprites;

    private Image _prevImg;

    private SceneController _sceneController;
    private ImageEffectController _effectController;

    private Color DEFAULT_COLOR = new Color(1.0f, 1.0f, 1.0f);

    private int _index = 0;

    private float _posX = 0;
    private float _posY = 0;
    private float _scaleX = 1;
    private float _scaleY = 1;
    private float _alphaValue = -1;
    private float _blurValue = 0;
    private float _duration = 0;

    private string _effect = "";

    private bool _isIndexChanged = false;
    private bool _isImageChanged = false;
    private bool _hasEffect = false;

    public bool isSkiping = false;

    void Start()
    {
        _effectController = GetComponent<ImageEffectController>();

        _sceneController = scene.GetComponent<SceneController>();

        _prevImg = prevImgObject.GetComponent<Image>();

        Initialize();
    }

    void LateUpdate()
    {
        UpdateImage();
        UpdateObjectTransform();
    }

    private void UpdateImage()
    {
        if(_isImageChanged)
        {
            if(_index == 0)
            {
                img.sprite = null;
            }
            else
            {
                if(_isIndexChanged)
                {
                    if(_hasEffect)
                    {
                        _prevImg.sprite = img.sprite;
                    }

                    img.sprite = sprites[_index - 1];
                    img.color = DEFAULT_COLOR;
                }
            }

            if(_hasEffect)
            {
                switch(_effect)
                {
                    case "FadeIn":
                    _sceneController.coroutineManager.StartCoroutineProcess(_effectController.FadeIn(_duration, _alphaValue > 0 ? _alphaValue : 0));
                    break;

                    case "FadeOut":
                    _sceneController.coroutineManager.StartCoroutineProcess(_effectController.FadeOut(_duration, _alphaValue > 0 ? _alphaValue : 1));
                    break;

                    case "Blur":
                    _sceneController.coroutineManager.StartCoroutineProcess(_effectController.Blur(_duration, _blurValue));
                    break;
                }
                ResetEffectProperties();
            }
            
            _isImageChanged = false;
        }
    }


    void ResetEffectProperties()
    {
        _hasEffect = false;
        _effect = "";
        _alphaValue = -1;
        _blurValue = 0;
        _duration = 0;
    }

    private void UpdateObjectTransform()
    {
        rt.anchoredPosition = new Vector2(_posX, _posY);
        rt.localScale = new Vector2(_scaleX, _scaleY);
    }

    public void Initialize()
    {
        img.sprite = null;
    }

    public void SetImageIndex(int index)
    {
        if (index < 0 && index >= sprites.Length)
        {
            throw new ArgumentException("잘못된 Image Index 사용");
        }

        if(_index != index)
        {
            _index = index;
            _isIndexChanged = true;
        }
        else
        {
            _isIndexChanged = false;
        }

        _isImageChanged = true;
    }

    public void SetImagePosition(float x, float y)
    {
        _posX = x;
        _posY = y;
    }

    public void SetImageScale(float x, float y)
    {
        _scaleX = x;
        _scaleY = y;
    }

    public void SetImageEffect(ImageData data)
    {
        _hasEffect = true;
        _effect = data.effect;
        _alphaValue = data.alphaValue;
        _blurValue = Mathf.Clamp(data.blurValue, 0f, 10f);
        _duration = data.effectDurationTime;
    }

    public IEnumerator SetImageData(ImageData data)
    {
        float originPosX = _posX;
        float originPosY = _posY;
        float originScaleX = _scaleX;
        float originScaleY = _scaleY;
        Color originColor = img.color;

        float dPosX = 0f;
        float dPosY = 0f;
        float dScaleX = 0f;
        float dScaleY = 0f;
        float dColorR = 0f;
        float dColorG = 0f;
        float dColorB = 0f;
        float dColorA = 0f;

        float proceedTime = 0f;
        float posDurationTime = data.posChangeTime;
        float scaleDurationTime = data.scaleChangeTime;
        float colorDurationTime = data.colorChangeTime;

        SetImageIndex(data.index);

        if(data.effect.Length > 0)
        {
           SetImageEffect(data);
        }


        if(posDurationTime > 0)
        {
            dPosX = data.posX - originPosX;
            dPosY = data.posY - originPosY;
        }
        else
        {
            _posX = data.posX > 0 ? data.posX : 0;
            _posY = data.posY > 0 ? data.posY : 0;
        }

        if(scaleDurationTime > 0)
        {
            dScaleX = data.scaleX - originScaleX;
            dScaleY = data.scaleY - originScaleY;
        }
        else
        {
            _scaleX = data.scaleX > 0 ? data.scaleX : 1;
            _scaleY = data.scaleY > 0 ? data.scaleY : 1;
        }

        if(data.color is not null)
        {
            Color dataColor = (Color)data.color;

            if(colorDurationTime > 0)
            {
                dColorR = dataColor.r - originColor.r;
                dColorG = dataColor.g - originColor.g;
                dColorB = dataColor.b - originColor.b;
                
                if(dataColor.a != originColor.a)
                {
                    dColorA = dataColor.a - originColor.a;
                }
            }
            else
            {
                img.color = dataColor;
            }
        }

        while(proceedTime < posDurationTime || proceedTime < scaleDurationTime || proceedTime < colorDurationTime)
        {
            proceedTime += Time.deltaTime;

            if(posDurationTime > 0)
            {
                if(proceedTime < posDurationTime)
                {
                    float nextPosX = originPosX + dPosX * Mathf.Clamp01(proceedTime / posDurationTime);
                    float nextPosY = originPosY + dPosY * Mathf.Clamp01(proceedTime / posDurationTime);

                    SetImagePosition(nextPosX, nextPosY);
                }
                else
                {
                    SetImagePosition(data.posX, data.posY);
                }
            }

            if(scaleDurationTime > 0)
            {
                if(proceedTime < scaleDurationTime)
                {
                    float nextScaleX = originScaleX + dScaleX * Mathf.Clamp01(proceedTime / scaleDurationTime);
                    float nextScaleY = originScaleY + dScaleY * Mathf.Clamp01(proceedTime / scaleDurationTime);

                    SetImageScale(nextScaleX, nextScaleY);
                }
                else
                {
                    SetImageScale(data.scaleX, data.scaleY);
                }
            }

            if(data.color is not null && colorDurationTime > 0)
            {
                Color dataColor = (Color)data.color;

                if(proceedTime < colorDurationTime)
                {
                    Color color;

                    float colorR = originColor.r + dColorR * Mathf.Clamp01(proceedTime / colorDurationTime);
                    float colorG = originColor.g + dColorG * Mathf.Clamp01(proceedTime / colorDurationTime);
                    float colorB = originColor.b + dColorB * Mathf.Clamp01(proceedTime / colorDurationTime);

                    if(dColorA > 0)
                    {
                        float colorA = originColor.a + dColorA * Mathf.Clamp01(proceedTime / colorDurationTime);

                        color = new Color(colorR, colorG, colorB, colorA);
                    }
                    else
                    {
                        color = new Color(colorR, colorG, colorB);
                    }

                    img.color = color;
                }
                else
                {
                    img.color = dataColor;
                }
            }

            yield return null;
        }
    }
}
