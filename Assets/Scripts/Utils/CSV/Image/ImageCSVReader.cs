using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ImageData
{
    private int COLOR_MAX_VALUE = 255;

    public int id;
    public int index;
    public string name;
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

    public ImageData(string[] data)
    {
        id = string.IsNullOrWhiteSpace(data[0]) ? -1 : Convert.ToInt16(data[0]);
        index = string.IsNullOrWhiteSpace(data[1]) ? -1 : Convert.ToInt16(data[1]);
        name = string.IsNullOrWhiteSpace(data[2]) ? "" : data[2];
        posX = string.IsNullOrWhiteSpace(data[3]) ? 0 : float.Parse(data[3]);
        posY = string.IsNullOrWhiteSpace(data[4]) ? 0 : float.Parse(data[4]);
        posChangeTime = string.IsNullOrWhiteSpace(data[5]) ? 0 : float.Parse(data[5]);
        scaleX = string.IsNullOrWhiteSpace(data[6]) ? -1 : float.Parse(data[6]);
        scaleY = string.IsNullOrWhiteSpace(data[7]) ? -1 : float.Parse(data[7]);
        scaleChangeTime = string.IsNullOrWhiteSpace(data[8]) ? 0 : float.Parse(data[8]);
        color = string.IsNullOrWhiteSpace(data[9]) ? null : ParseColor(data[9]);
        colorChangeTime = string.IsNullOrWhiteSpace(data[10]) ? 0 : float.Parse(data[10]);
        effect = string.IsNullOrWhiteSpace(data[11]) ? "" : data[11];
        effectDurationTime = string.IsNullOrWhiteSpace(data[12]) ? 0 : float.Parse(data[12]);
        alphaValue = string.IsNullOrWhiteSpace(data[13]) ? -1 : float.Parse(data[13]);
        blurValue = string.IsNullOrWhiteSpace(data[14]) ? 0 : float.Parse(data[14]);
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

public class ImageCSVReader : MonoBehaviour
{
    private int _dataLength;

    public ImageData[] LoadData(CSVLoader csvLoader, string filePath)
    {
        List<string[]> imageCSVData = csvLoader.LoadCSV(filePath);

        List<ImageData> imageData = new List<ImageData>();

        for (int index = 0; index < imageCSVData.Count; index++)
        {
            if (index == 0)
            {
                _dataLength = imageCSVData[index].Length;
                continue;
            }

            string[] rawData = imageCSVData[index];

            if (rawData.Length != _dataLength)
            {
                Debug.LogError($"{index}번째 Image 데이터 에러");
            }

            ImageData imageDatum = new ImageData(rawData);

            imageData.Add(imageDatum);
        }

        return imageData.ToArray();
    }
}
