using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptData
{
    public int id;
    public int dialogueId;
    public int choiceId;
    public int spriteId;
    public int itemId;
    public int backgroundId;
    public int foregroundId;
    public int sceneId;
    public int audioBGMId;
    public float audioBGMDelay;
    public int audioSFXId;
    public float audioSFXDelay;
    public bool autoPass;
    public bool blockInteraction;

    public ScriptData(string[] data)
    {
        id = string.IsNullOrWhiteSpace(data[0]) ? -1 : Convert.ToInt16(data[0]);
        dialogueId = string.IsNullOrWhiteSpace(data[1]) ? 0 : Convert.ToInt16(data[1]);
        choiceId = string.IsNullOrWhiteSpace(data[2]) ? -1 : Convert.ToInt16(data[2]);
        spriteId = string.IsNullOrWhiteSpace(data[3]) ? -1 : Convert.ToInt16(data[3]);
        itemId = string.IsNullOrWhiteSpace(data[4]) ? -1 : Convert.ToInt16(data[4]);
        backgroundId = string.IsNullOrWhiteSpace(data[5]) ? -1 : Convert.ToInt16(data[5]);
        foregroundId = string.IsNullOrWhiteSpace(data[6]) ? -1 : Convert.ToInt16(data[6]);
        sceneId = string.IsNullOrWhiteSpace(data[7]) ? -1 : Convert.ToInt16(data[7]);
        audioBGMId = string.IsNullOrWhiteSpace(data[8]) ? -1 : Convert.ToInt16(data[8]);
        audioSFXId = string.IsNullOrWhiteSpace(data[9]) ? -1 : Convert.ToInt16(data[9]);
        autoPass = string.IsNullOrWhiteSpace(data[10]) ? false : true;
        blockInteraction = string.IsNullOrWhiteSpace(data[11]) ? false : true;
    }
}

public class ScriptCSVReader : MonoBehaviour
{
    private int _dataLength;

    public ScriptData[] LoadData(CSVLoader csvLoader, string filePath)
    {
        List<string[]> scriptCSVData = csvLoader.LoadCSV(filePath);

        List<ScriptData> scriptData = new List<ScriptData>();

        for (int index = 0; index < scriptCSVData.Count; index++)
        {
            if (index == 0)
            {
                _dataLength = scriptCSVData[index].Length;
                continue;
            }

            string[] rawData = scriptCSVData[index];

            if (rawData.Length != _dataLength)
            {
                Debug.LogError($"{index}번째 script 데이터 에러");
            }

            ScriptData scriptDatum = new ScriptData(rawData);

            scriptData.Add(scriptDatum);
        }

        return scriptData.ToArray();
    }
}
