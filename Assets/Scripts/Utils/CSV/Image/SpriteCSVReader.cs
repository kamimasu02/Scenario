using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpriteData
{
    private int COLOR_MAX_VALUE = 255;

    public int id;
    public int groupId;
    public string name;
    public int index;
    public float posX;
    public float posY;
    public float posChangeTime;
    public float scaleX;
    public float scaleY;
    public float scaleChangeTime;
    public Color? color;
    public float colorChangeTime;
    public string effect;
    public float effectDurationTime;
    public float alphaValue;
    public float blurValue;
    public bool flip;
    public string animationState;
    public int iconId;
    public string tag;

    public SpriteData(string[] data)
    {
        id = string.IsNullOrWhiteSpace(data[0]) ? -1 : Convert.ToInt16(data[0]);
        groupId = string.IsNullOrWhiteSpace(data[1]) ? -1 : Convert.ToInt16(data[1]);
        name = string.IsNullOrWhiteSpace(data[2]) ? "" : data[2];
        index = string.IsNullOrWhiteSpace(data[3]) ? 0 : Convert.ToInt16(data[3]);
        posX = string.IsNullOrWhiteSpace(data[4]) ? Single.NaN : float.Parse(data[4]);
        posY = string.IsNullOrWhiteSpace(data[5]) ? Single.NaN : float.Parse(data[5]);
        posChangeTime = string.IsNullOrWhiteSpace(data[6]) ? 0 : float.Parse(data[6]);
        scaleX = string.IsNullOrWhiteSpace(data[7]) ? -1 : float.Parse(data[7]);
        scaleY = string.IsNullOrWhiteSpace(data[8]) ? -1 : float.Parse(data[8]);
        scaleChangeTime = string.IsNullOrWhiteSpace(data[9]) ? 0 : float.Parse(data[9]);
        color = string.IsNullOrWhiteSpace(data[10]) ? null : ParseColor(data[10]);
        colorChangeTime = string.IsNullOrWhiteSpace(data[11]) ? 0 : float.Parse(data[11]);
        effect = string.IsNullOrWhiteSpace(data[12]) ? "" : data[12];
        effectDurationTime = string.IsNullOrWhiteSpace(data[13]) ? 0 : float.Parse(data[13]);
        alphaValue = string.IsNullOrWhiteSpace(data[14]) ? -1 : float.Parse(data[14]);
        blurValue = string.IsNullOrWhiteSpace(data[15]) ? -1 : float.Parse(data[15]);
        flip = string.IsNullOrWhiteSpace(data[16]) ? false : true;
        animationState = string.IsNullOrWhiteSpace(data[17]) ? "" : data[17];
        iconId = string.IsNullOrWhiteSpace(data[18]) ? -1 : Convert.ToInt16(data[18]);
        tag = string.IsNullOrWhiteSpace(data[19]) ? "" : data[19];
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

public class SpriteCSVReader : MonoBehaviour
{
    private int _dataLength;

    public SpriteData[] LoadData(CSVLoader csvLoader, string filePath)
    {
        List<string[]> spriteCSVData = csvLoader.LoadCSV(filePath);

        List<SpriteData> spriteData = new List<SpriteData>();

        for (int index = 0; index < spriteCSVData.Count; index++)
        {
            if (index == 0)
            {
                _dataLength = spriteCSVData[index].Length;
                continue;
            }

            string[] rawData = spriteCSVData[index];

            if (rawData.Length != _dataLength)
            {
                Debug.LogError($"{index}번째 Sprite 데이터 에러");
            }

            SpriteData spriteDatum = new SpriteData(rawData);

            spriteData.Add(spriteDatum);
        }

        return spriteData.ToArray();
    }
}
