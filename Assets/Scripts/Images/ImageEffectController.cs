using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageEffectController : MonoBehaviour
{
    [SerializeField] GameObject prevImgObject;

    [SerializeField] Image img;

    private Image _prevImg;
    private Material _blurMaterial;

    void Start()
    {
        Shader blurShader = Resources.Load<Shader>("Shaders/BlurShader");

        _blurMaterial = new Material(blurShader);

        _prevImg = prevImgObject.GetComponent<Image>();
    }

    public IEnumerator Blur(float durationTime, float blurValue)
    {
        if(blurValue > 0)
        {
            float proceedTime = 0f;
            
            img.material = _blurMaterial;

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

            img.material = null;
        }
    }

    public IEnumerator FadeIn(float durationTime, float alpha = 0, Color? nullableColor = null)
    {
        if (nullableColor is Color targetColor)
        {
            img.color = targetColor;
        }

        Color color = img.color;
        color.a = 0f;

        float proceedTime = 0f;

        while (proceedTime < durationTime)
        {
            proceedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(1 - (proceedTime / durationTime));
            img.color = color;
            yield return null;
        }

        color.a = 0f;
        img.color = color;
        _prevImg.sprite = null;
    }

     public IEnumerator FadeOut(float durationTime, float alpha = 1, Color? nullableColor = null)
    {
        if (nullableColor is Color targetColor)
        {
            img.color = targetColor;
        }

        Color color = img.color;
        color.a = 1f;

        float proceedTime = 0f;

        while (proceedTime < durationTime)
        {
            proceedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(proceedTime / durationTime);
            img.color = color;
            yield return null;
        }

        color.a = 1f;
        img.color = color;
        _prevImg.sprite = null;
    }
}
