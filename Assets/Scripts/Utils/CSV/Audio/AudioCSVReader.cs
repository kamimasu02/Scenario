using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioData
{
    private int VOLUME_MAX_VALUE = 100;

    public int id;
    public int index;
    public string name;
    public bool loop;
    public float volume;
    public string effect;
    public float effectDurationTime;
    public float startAt;
    public float endAt;
    public float delay;

    public AudioData(string[] data)
    {
        id = string.IsNullOrWhiteSpace(data[0]) ? -1 : Convert.ToInt16(data[0]);
        index = string.IsNullOrWhiteSpace(data[1]) ? -1 : Convert.ToInt16(data[1]);
        name = string.IsNullOrWhiteSpace(data[2]) ? "" : data[2];
        loop = string.IsNullOrWhiteSpace(data[3]) ? false : true;
        volume = string.IsNullOrWhiteSpace(data[4]) ? 1.0f : Mathf.Clamp01((float) Convert.ToInt16(data[4]) / VOLUME_MAX_VALUE);
        effect = string.IsNullOrWhiteSpace(data[5]) ? "" : data[5];
        effectDurationTime = string.IsNullOrWhiteSpace(data[6]) ? 0 : float.Parse(data[6]);
        startAt = string.IsNullOrWhiteSpace(data[7]) ? 0 : float.Parse(data[7]);
        endAt = string.IsNullOrWhiteSpace(data[8]) ? 0 : float.Parse(data[8]);
        delay = string.IsNullOrWhiteSpace(data[9]) ? 0 : float.Parse(data[9]);
    }
}

public class AudioCSVReader : MonoBehaviour
{
    private int _dataLength;

    public AudioData[] LoadData(CSVLoader csvLoader, string filePath)
    {
        List<string[]> audioCSVData = csvLoader.LoadCSV(filePath);

        List<AudioData> audioData = new List<AudioData>();

        for (int index = 0; index < audioCSVData.Count; index++)
        {
            if (index == 0)
            {
                _dataLength = audioCSVData[index].Length;
                continue;
            }

            string[] rawData = audioCSVData[index];

            if (rawData.Length != _dataLength)
            {
                Debug.LogError($"{index}번째 Audio 데이터 에러");
            }

            AudioData audioDatum = new AudioData(rawData);

            audioData.Add(audioDatum);
        }

        return audioData.ToArray();
    }
}
