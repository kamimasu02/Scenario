using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioOption
{
    public float volume = 0.5f;
    public bool isMute = false;
}

public class AudioController : MonoBehaviour
{
    [SerializeField] GameObject scene;
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip[] audios;

    private SceneController _sceneController;
    private AudioEffectController _effectController;
    private AudioOption _option = new AudioOption();

    void Start()
    {
        _sceneController = scene.GetComponent<SceneController>();

        _effectController = GetComponent<AudioEffectController>();
    }

    public void Play(AudioData data)
    {
        source.clip = audios[data.index - 1];
        source.loop = data.loop;
        source.time = data.startAt;
        source.mute = _option.isMute;

        if(source.isPlaying)
        {
            source.Stop();
        }

        if(data.effect.Length > 0)
        {
            source.volume = 0;

            switch(data.effect)
            {
                case "FadeIn":
                if(data.delay == 0)
                {
                    _sceneController.coroutineManager.StartCoroutineProcess(_effectController.FadeIn(data.effectDurationTime, data.volume));
                }
                else
                {
                    _sceneController.coroutineManager.StartCoroutineProcess(_effectController.FadeIn(data.effectDurationTime, data.volume, data.delay));
                }
                break;
            }
        }
        else
        {
            source.volume = data.volume > 0 ? Mathf.Clamp01(data.volume * _option.volume) : _option.volume;

            if(data.delay == 0)
            {
                source.Play();
            }
            else
            {
                _sceneController.coroutineManager.StartCoroutineProcess(PlayWithDelay(data.delay));
            }
        }
    }

    #nullable enable
    public void Stop(AudioData? data = null)
    {
        if(data is not null)
        {
            if(data.effect.Length > 0)
            {
                switch(data.effect)
                {
                    case "FadeOut":
                    _sceneController.coroutineManager.StartCoroutineProcess(_effectController.FadeOut(data.effectDurationTime));
                    break;
                }
            }
            else
            {
                source.Stop();
            }
        }
        else
        {
            source.Stop();
        }
    }
    #nullable disable

    IEnumerator PlayWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        source.Play();
    }

    public void SetOption(float volume, bool isMute)
    {
        _option.volume = volume;
        _option.isMute = isMute;
    }

    public AudioOption GetOption()
    {
        return _option;
    }
}
