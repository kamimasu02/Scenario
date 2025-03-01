using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteController : MonoBehaviour
{
    [SerializeField] GameObject scene;
    [SerializeField] GameObject spriteObject;
    [SerializeField] GameObject prevSpriteObject;
    [SerializeField] GameObject iconObject;


    [Header("Animation")]
    [SerializeField] Animator animator;


    [Header("Sprite")]
    [SerializeField] Sprite[] sprites;

    private SpriteRenderer _spriteRenderer;
    private RectTransform _rectTransform;
    private SpriteRenderer _prevSpriteRenderer;
    private RectTransform _prevRectTransform;

    private SceneController _sceneController;
    private SpriteEffectController _effectController;
    private IconController _iconController;

    private Color DEFAULT_COLOR = new Color(1.0f, 1.0f, 1.0f);

    private int _index = 0;

    private float _posX;
    private float _posY;
    private float _scaleX;
    private float _scaleY;
    private float _alphaValue = -1;
    private float _blurValue = 0;
    private float _duration = 0;

    private string _tag;
    private string _animationState = "";
    private string _effect = "";

    private bool _isIndexChanged = false;
    private bool _isSpriteChanged = false;
    private bool _hasEffect = false;

    public bool isSkiping = false;

    void Start()
    {
        _effectController = GetComponent<SpriteEffectController>();

        _sceneController = scene.GetComponent<SceneController>();
        _iconController = iconObject.GetComponent<IconController>();

        _rectTransform = spriteObject.GetComponent<RectTransform>();
        _spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();
        _prevRectTransform = prevSpriteObject.GetComponent<RectTransform>();
        _prevSpriteRenderer = prevSpriteObject.GetComponent<SpriteRenderer>();

        Color color = _spriteRenderer.color;
        _spriteRenderer.color = color;
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.normalizedTime >= 1.0f && _animationState.EndsWith("Once"))
        {
            if (stateInfo.IsName(_animationState))
            {
                SetBoolAnimatorParameter(_animationState, false);
            }
        }
    }

    void LateUpdate()
    {
        UpdateSprite();

        if(_animationState.Length == 0)
        {
            UpdateObjectTransform();
        }
    }

    private void UpdateSprite()
    {
        if(_isSpriteChanged)
        {
            if(_spriteRenderer.color.a == 0)
            {
                _spriteRenderer.color = DEFAULT_COLOR;                
            }

            if(_index == 0)
            {
                if(_hasEffect)
                {
                    _prevRectTransform.anchoredPosition = _rectTransform.anchoredPosition;
                    _prevRectTransform.rotation = _rectTransform.rotation;
                    _prevRectTransform.localScale = _rectTransform.localScale;

                    _prevSpriteRenderer.sprite = _spriteRenderer.sprite;
                }

                _spriteRenderer.sprite = null;
            }
            else
            {
                if(_isIndexChanged)
                {
                    if(_hasEffect)
                    {
                        _prevRectTransform.anchoredPosition = _rectTransform.anchoredPosition;
                        _prevRectTransform.rotation = _rectTransform.rotation;
                        _prevRectTransform.localScale = _rectTransform.localScale;

                        _prevSpriteRenderer.sprite = _spriteRenderer.sprite;
                    }
                    
                    _spriteRenderer.sprite = sprites[_index - 1];
                }
            }

            if(_hasEffect)
            {
                switch(_effect)
                {
                    case "FadeIn":
                    _sceneController.coroutineManager.StartCoroutineProcess(_effectController.FadeIn(_duration, _alphaValue >= 0 ? _alphaValue : 0));
                    break;

                    case "FadeOut":
                    _sceneController.coroutineManager.StartCoroutineProcess(_effectController.FadeOut(_duration, _alphaValue >= 0 ? _alphaValue : 1));
                    break;

                    case "Blur":
                    _sceneController.coroutineManager.StartCoroutineProcess(_effectController.Blur(_duration, _blurValue));
                    break;
                }
                ResetEffectProperties();
            }

            _isSpriteChanged = false;
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
        _rectTransform.anchoredPosition = new Vector2(_posX, _posY);
        _rectTransform.localScale = new Vector2(_scaleX, _scaleY);
    }

    public void Initialize(CharacterData data)
    {
        _tag = data.tag;

        SetSpriteIndex(data.index);
        SetSpritePosition(data.posX, data.posY);
        SetSpriteScale(data.scaleX, data.scaleY);
        // SetObjectActive(data.isActive);
    }

    public void SetSpriteIndex(int index)
    {
        if (index < 0 && index >= sprites.Length)
        {
            throw new ArgumentException("잘못된 Sprite Index 사용");
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

        _isSpriteChanged = true;
    }

    public void SetSpritePosition(float x, float y)
    {
        _posX = x;
        _posY = y;
    }

    public void SetSpriteScale(float x, float y)
    {
        _scaleX = x;
        _scaleY = y;
    }

    public void SetSpriteEffect(SpriteData data)
    {
        _hasEffect = true;
        _effect = data.effect;
        _alphaValue = data.alphaValue;
        _blurValue = Mathf.Clamp(data.blurValue, 0f, 10f);
        _duration = data.effectDurationTime;
    }

    public void SetObjectActive(bool activeValue)
    {
        Color color = _spriteRenderer.color;

        if(activeValue)
        {
            color.a = 1f;
        }
        else
        {
            color.a = 0f;
        }

        _spriteRenderer.color = color;
    }

    public void SetBoolAnimatorParameter(string animationState, bool boolValue)
    {
        animator.SetBool($"is{animationState}", boolValue);

        if (boolValue)
        {
            _animationState = animationState;
        }
        else
        {
            _animationState = "";
        }
    }

    public bool GetBoolAnimatorParameter(string animationState)
    {
        return animator.GetBool($"is{animationState}");
    }

    public void SetFloatAnimatorParameter(string parameterKey, float intValue)
    {
        animator.SetFloat(parameterKey, intValue);
    }

    public float GetFloatAnimatorParameter(string parameterKey)
    {
        return animator.GetFloat(parameterKey);
    }

    #nullable enable
    public IEnumerator SetSpriteData(SpriteData data, IconData? icon = null)
    {
        float originPosX = _posX;
        float originPosY = _posY;
        float originScaleX = _scaleX;
        float originScaleY = _scaleY;
        Color originColor = _spriteRenderer.color;

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

        SetSpriteIndex(data.index);

        if(data.effect.Length > 0)
        {
            SetSpriteEffect(data);
        }

        if(data.animationState.Length > 0)
        {
            if(data.animationState != "Idle")
            {
                SetBoolAnimatorParameter(data.animationState, true);
            }
            else
            {
                if(_animationState.Length > 0)
                {
                    SetBoolAnimatorParameter(_animationState, false);
                }
            }
        }

        if(icon is not null)
        {
            _iconController.SetIconData(icon);
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
            if(data.scaleX > 0)
            {
                _scaleX = data.scaleX;
            }

            if(data.scaleY > 0)
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
                _spriteRenderer.color = dataColor;
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

                    SetSpritePosition(nextPosX, nextPosY);
                }
                else
                {
                    SetSpritePosition(data.posX, data.posY);
                }
            }

            if(scaleDurationTime > 0)
            {
                if(proceedTime < scaleDurationTime)
                {
                    float nextScaleX = originScaleX + dScaleX * Mathf.Clamp01(proceedTime / scaleDurationTime);
                    float nextScaleY = originScaleY + dScaleY * Mathf.Clamp01(proceedTime / scaleDurationTime);

                    SetSpriteScale(nextScaleX, nextScaleY);
                }
                else
                {
                    SetSpriteScale(data.scaleX, data.scaleY);
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

                    _spriteRenderer.color = color;
                }
                else
                {
                    _spriteRenderer.color = dataColor;
                }
            }

            yield return null;
        }
    }
    #nullable disable
}
