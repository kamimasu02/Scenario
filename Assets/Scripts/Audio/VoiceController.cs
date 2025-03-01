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

    [SerializeField] GameObject sceneObject;
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip[] voices;

    private SceneController _sceneController;
    private VoiceOption _option = new VoiceOption();

    void Start()
    {
        _sceneController = sceneObject.GetComponent<SceneController>();

        OptionController.VoiceOptionChange += UpdateVoiceOption;
    }

    public void Play(int index, float delay = 0)
    {
        source.clip = voices[index - 1];

        if(delay == 0)
        {
            source.Play();
        }
        else
        {
            _sceneController.coroutineManager.StartCoroutineProcess(PlayWithDelay(source, delay));
        }
    }

    public void Stop()
    {
        source.Stop();
    }

    IEnumerator PlayWithDelay(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        source.Play();
    }

    void UpdateVoiceOption(float volume, bool isMute)
    {
        _option.volume = volume;
        _option.isMute = isMute;
        if(source is not null)
        {
            source.volume = _option.volume;
            source.mute = _option.isMute;
        }
    }

    void OnDestory()
    {
        OptionController.VoiceOptionChange -= UpdateVoiceOption;
    }
}