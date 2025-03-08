using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEffectController : MonoBehaviour
{
    [SerializeField] private SceneController _sceneController;
    [SerializeField] private AudioController _audioController;
    [SerializeField] private AudioSource _source;

    private AudioOption _options;

    void Start()
    {
        _options = _audioController.GetOption();
    }

    public IEnumerator FadeIn(float durationTime, float volume = 1f, float delay = 0)
    {
        if(delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }

        _source.Play();

        float targetVolume = _options.volume * volume;

        float proceedTime = 0f;

        while (proceedTime < durationTime)
        {
            proceedTime += Time.deltaTime;
            _source.volume = targetVolume * Mathf.Clamp01(proceedTime / durationTime);
            yield return null;
        }

        _source.volume = targetVolume;   
    }

    public IEnumerator FadeOut(float durationTime)
    {
        float originVolume = _source.volume;

        float proceedTime = 0f;

        while (proceedTime < durationTime)
        {
            proceedTime += Time.deltaTime;
            _source.volume = originVolume * Mathf.Clamp01(1 - (proceedTime / durationTime));
            yield return null;
        }

        _source.volume = 0f;
        _source.Stop();
    }
}
