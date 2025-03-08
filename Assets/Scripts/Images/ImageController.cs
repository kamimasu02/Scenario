using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageController : MonoBehaviour
{
    [Header("Transform")]
    [SerializeField] private RectTransform _rt;

    [Header("Image")]
    [SerializeField] private Image _img;
    [SerializeField] private Image _prevImg;
    [SerializeField] private Sprite[] _sprites;
    
    [Header("Scene")]
    [SerializeField] private SceneController _sceneController;
    [SerializeField] private ImageEffectController _effectController;

    private readonly Color _DEFAULT_COLOR = new Color(1.0f, 1.0f, 1.0f);

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
                _img.sprite = null;
            }
            else
            {
                if(_isIndexChanged)
                {
                    if(_hasEffect)
                    {
                        _prevImg.sprite = _img.sprite;
                    }

                    _img.sprite = _sprites[_index - 1];
                    _img.color = _DEFAULT_COLOR;
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
        _rt.anchoredPosition = new Vector2(_posX, _posY);
        _rt.localScale = new Vector2(_scaleX, _scaleY);
    }

    public void Initialize()
    {
        _img.sprite = null;
        _prevImg.sprite = null;
    }

    public void SetImageIndex(int index)
    {
        if (index < 0 && index >= _sprites.Length)
        {
            throw new ArgumentException("잘못된 Image Index 사용");
        }

        if(index >= 0 && _index != index)
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
        Color originColor = _img.color;

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
            if(!float.IsNaN(data.posX))
            {
                _posX = data.posX;
            }

            if(!float.IsNaN(data.posY))
            {
                _posY = data.posY;
            }
        }

        if(scaleDurationTime > 0)
        {
            dScaleX = data.scaleX - originScaleX;
            dScaleY = data.scaleY - originScaleY;
        }
        else
        {
            if(!float.IsNaN(data.scaleX))
            {
                _scaleX = data.scaleX;
            }

            if(!float.IsNaN(data.scaleY))
            {
                _scaleY = data.scaleY;
            }
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
                _img.color = dataColor;
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

                    _img.color = color;
                }
                else
                {
                    _img.color = dataColor;
                }
            }

            yield return null;
        }
    }
}
