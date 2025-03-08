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
    [SerializeField] private SceneController _sceneController;
    [SerializeField] private AudioEffectController _effectController;
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip[] _audios;

    private AudioOption _option = new AudioOption();

    public void Play(AudioData data)
    {
        _source.clip = _audios[data.index - 1];
        _source.loop = data.loop;
        _source.time = data.startAt;
        _source.mute = _option.isMute;

        if(_source.isPlaying)
        {
            _source.Stop();
        }

        if(data.effect.Length > 0)
        {
            _source.volume = 0;

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
            _source.volume = data.volume > 0 ? Mathf.Clamp01(data.volume * _option.volume) : _option.volume;

            if(data.delay == 0)
            {
                _source.Play();
            }
            else
            {
                _sceneController.coroutineManager.StartCoroutineProcess(PlayWithDelay(data.delay));
            }
        }
    }

    #nullable enable
    public void SetData(AudioData? data)
    {
        if(data is not null)
        {
            if(data.index > 0)
            {
                Play(data);
            }
            else if(data.index == 0)
            {
                Stop(data);
            }
        }
        else
        {
            Stop();
        }
    }

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
                _source.Stop();
            }
        }
        else
        {
            _source.Stop();
        }
    }
    #nullable disable

    IEnumerator PlayWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _source.Play();
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
