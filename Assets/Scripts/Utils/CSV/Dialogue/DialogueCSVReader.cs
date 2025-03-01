using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueData
{
    private int COLOR_MAX_VALUE = 255;
    private Color DEFAULT_COLOR = new Color(1.0f, 1.0f, 1.0f);

    public int id;
    public string name;
    public string text;
    public float typeSpeed;
    public string fontAsset;
    public string fontStyle;
    public int fontSize;
    public Color fontColor;
    public int voiceIndex;
    public string tag;

    public DialogueData(string[] data)
    {
        id = string.IsNullOrWhiteSpace(data[0]) ? -1 : Convert.ToInt16(data[0]);
        name = string.IsNullOrWhiteSpace(data[1]) ? "" : data[1];
        text = string.IsNullOrWhiteSpace(data[2]) ? "" : data[2];
        typeSpeed = string.IsNullOrWhiteSpace(data[3]) ? 0 : float.Parse(data[3]);
        fontAsset = string.IsNullOrWhiteSpace(data[4]) ? "" : data[4];
        fontStyle = string.IsNullOrWhiteSpace(data[5]) ? "" : data[5];
        fontSize = string.IsNullOrWhiteSpace(data[6]) ? -1 : Convert.ToInt16(data[6]);
        fontColor = string.IsNullOrWhiteSpace(data[7]) ? DEFAULT_COLOR : ParseColor(data[7]);
        voiceIndex = string.IsNullOrWhiteSpace(data[8]) ? -1 : Convert.ToInt16(data[8]);
        tag = string.IsNullOrWhiteSpace(data[9]) ? "" : data[9];
    }

    Color ParseColor(string color)
    {
        string[] colorRGBA = color.Split('.');

        if(colorRGBA.Length != 3 && colorRGBA.Length != 4)
        {
            Debug.LogError("Color 인자 에러");
        }

        float colorR = string.IsNullOrWhiteSpace(colorRGBA[0]) ? 0 : Mathf.Clamp01((float) Convert.ToInt16(colorRGBA[0]) / COLOR_MAX_VALUE);
        float colorG = string.IsNullOrWhiteSpace(colorRGBA[1]) ? 0 : Mathf.Clamp01((float) Convert.ToInt16(colorRGBA[1]) / COLOR_MAX_VALUE);
        float colorB = string.IsNullOrWhiteSpace(colorRGBA[2]) ? 0 : Mathf.Clamp01((float) Convert.ToInt16(colorRGBA[2]) / COLOR_MAX_VALUE);

        if(colorRGBA.Length == 3)
        {
            return new Color(colorR,colorG,colorB);   
        }

        float colorA = string.IsNullOrWhiteSpace(colorRGBA[3]) ? 0 : Mathf.Clamp01((float) Convert.ToInt16(colorRGBA[3]) / COLOR_MAX_VALUE);

        return new Color(colorR, colorG, colorB, colorA);
    }
}

public class DialogueCSVReader : MonoBehaviour
{
    private int _dataLength;

    public DialogueData[] LoadData(CSVLoader csvLoader, string filePath)
    {
        List<string[]> dialogueCSVData = csvLoader.LoadCSV(filePath);

        List<DialogueData> dialogueData = new List<DialogueData>();

        for (int index = 0; index < dialogueCSVData.Count; index++)
        {
            if (index == 0)
            {
                _dataLength = dialogueCSVData[index].Length;
                continue;
            }

            string[] rawData = dialogueCSVData[index];

            if (rawData.Length != _dataLength)
            {
                Debug.LogError($"{index}번째 Dialogue 데이터 에러");
            }

            DialogueData dialogueDatum = new DialogueData(rawData);

            dialogueData.Add(dialogueDatum);
        }

        return dialogueData.ToArray();
    }
}
