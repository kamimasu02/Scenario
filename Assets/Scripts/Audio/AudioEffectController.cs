using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEffectController : MonoBehaviour
{
    [SerializeField] GameObject scene;
    [SerializeField] AudioSource source;

    private SceneController _sceneController;
    private AudioController _audioController;
    private AudioOption _options;

    void Start()
    {
        _sceneController = scene.GetComponent<SceneController>();

        _audioController = GetComponent<AudioController>();
        _options = _audioController.GetOption();
    }

    public IEnumerator FadeIn(float durationTime, float volume = 1f, float delay = 0)
    {
        if(delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }

        source.Play();

        float targetVolume = _options.volume * volume;

        float proceedTime = 0f;

        while (proceedTime < durationTime)
        {
            proceedTime += Time.deltaTime;
            source.volume = targetVolume * Mathf.Clamp01(proceedTime / durationTime);
            yield return null;
        }

        source.volume = targetVolume;   
    }

     public IEnumerator FadeOut(float durationTime)
    {
        float originVolume = source.volume;

        float proceedTime = 0f;

        while (proceedTime < durationTime)
        {
            proceedTime += Time.deltaTime;
            source.volume = originVolume * Mathf.Clamp01(1 - (proceedTime / durationTime));
            yield return null;
        }

        source.volume = 0f;
        source.Stop();
    }
}
