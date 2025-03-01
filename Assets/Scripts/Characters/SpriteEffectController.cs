using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteEffectController : MonoBehaviour
{
    [SerializeField] GameObject spriteObject;
    [SerializeField] GameObject prevSpriteObject;

    private SpriteRenderer _spriteRenderer;
    private SpriteRenderer _prevSpriteRenderer;
    private Material _blurMaterial;

    void Start()
    {
        Shader blurShader = Resources.Load<Shader>("Shaders/BlurShader");

        _blurMaterial = new Material(blurShader);

        _spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();
        _prevSpriteRenderer = prevSpriteObject.GetComponent<SpriteRenderer>();
    }

    public IEnumerator Blur(float durationTime, float blurValue)
    {
        if(blurValue > 0)
        {
            float proceedTime = 0f;
            
            _spriteRenderer.material = _blurMaterial;

            while (proceedTime < durationTime)
            {
                proceedTime += Time.deltaTime;
                float currentBlurValue = Mathf.Clamp(blurValue * (proceedTime / durationTime), 0f, 10f);
                _blurMaterial.SetFloat("_BlurSize", currentBlurValue);
                yield return null;
            }

            _blurMaterial.SetFloat("_BlurSize", blurValue);
        }
        else
        {
            _blurMaterial.SetFloat("_BlurSize", blurValue);

            _spriteRenderer.material = null;
        }
    }

    public IEnumerator FadeIn(float durationTime, float alpha = 0, Color? nullableColor = null)
    {
        if (nullableColor is Color targetColor)
        {
            _spriteRenderer.color = targetColor;
        }

        Color color = _spriteRenderer.color;
        color.a = alpha;

        float proceedTime = 0f;

        while (proceedTime < durationTime)
        {
            proceedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(alpha * (proceedTime / durationTime));
            _prevSpriteRenderer.color = color;
            color.a = Mathf.Clamp01(alpha * (1 - (proceedTime / durationTime)));
            _spriteRenderer.color = color;
            yield return null;
        }

        color.a = alpha;
        _spriteRenderer.color = color;
        _prevSpriteRenderer.sprite = null;
    }

     public IEnumerator FadeOut(float durationTime, float alpha = 1, Color? nullableColor = null)
    {
        if (nullableColor is Color targetColor)
        {
            _spriteRenderer.color = targetColor;
        }

        Color color = _spriteRenderer.color;
        color.a = alpha;

        float proceedTime = 0f;

        while (proceedTime < durationTime)
        {
            proceedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(alpha * (1 - (proceedTime / durationTime)));
            _prevSpriteRenderer.color = color;
            color.a = Mathf.Clamp01(alpha * proceedTime / durationTime);
            _spriteRenderer.color = color;
            yield return null;
        }

        color.a = alpha;
        _spriteRenderer.color = color;
        _prevSpriteRenderer.sprite = null;
    }
}
