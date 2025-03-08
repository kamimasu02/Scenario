using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceController : MonoBehaviour
{
    public class VoiceOption
    {
        public float volume = 0.5f;
        public bool isMute = false;
    }

    [SerializeField] private SceneController _sceneController;
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip[] _voices;

    private VoiceOption _option = new VoiceOption();

    void Start()
    {
        OptionController.VoiceOptionChange += UpdateVoiceOption;
    }

    public void Play(int index, float delay = 0)
    {
        _source.clip = _voices[index - 1];

        if(delay == 0)
        {
            _source.Play();
        }
        else
        {
            _sceneController.coroutineManager.StartCoroutineProcess(PlayWithDelay(delay));
        }
    }

    public void Stop()
    {
        _source.Stop();
    }

    IEnumerator PlayWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _source.Play();
    }

    void UpdateVoiceOption(float volume, bool isMute)
    {
        _option.volume = volume;
        _option.isMute = isMute;
        if(_source is not null)
        {
            _source.volume = _option.volume;
            _source.mute = _option.isMute;
        }
    }

    void OnDestory()
    {
        OptionController.VoiceOptionChange -= UpdateVoiceOption;
    }
}