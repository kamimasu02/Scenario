using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData
{
    public int id;
    public string name;
    public int index;
    public float posX;
    public float posY;
    public float scaleX;
    public float scaleY;
    public bool isActive;
    public string tag;

    public CharacterData(string[] data)
    {
        id = string.IsNullOrWhiteSpace(data[0]) ? -1 : Convert.ToInt16(data[0]);
        name = string.IsNullOrWhiteSpace(data[1]) ? "" : data[1];
        index = string.IsNullOrWhiteSpace(data[2]) ? 0 : Convert.ToInt16(data[2]);
        posX = string.IsNullOrWhiteSpace(data[3]) ? Single.NaN : float.Parse(data[3]);
        posY = string.IsNullOrWhiteSpace(data[4]) ? Single.NaN : float.Parse(data[4]);
        scaleX = string.IsNullOrWhiteSpace(data[5]) ? 1 : float.Parse(data[5]);
        scaleY = string.IsNullOrWhiteSpace(data[6]) ? 1 : float.Parse(data[6]);
        isActive = string.IsNullOrWhiteSpace(data[7]) ? false : true;
        tag = string.IsNullOrWhiteSpace(data[8]) ? "" : data[8];
    }
}

public class CharacterCSVReader : MonoBehaviour
{
    private int _dataLength;

    public CharacterData[] LoadData(CSVLoader csvLoader, string filePath)
    {
        List<string[]> charCSVData = csvLoader.LoadCSV(filePath);

        List<CharacterData> charData = new List<CharacterData>();

        for (int index = 0; index < charCSVData.Count; index++)
        {
            if (index == 0)
            {
                _dataLength = charCSVData[index].Length;
                continue;
            }

            string[] rawData = charCSVData[index];

            if (rawData.Length != _dataLength)
            {
                Debug.LogError($"{index}번째 char 데이터 에러");
            }

            CharacterData charDatum = new CharacterData(rawData);

            charData.Add(charDatum);
        }

        return charData.ToArray();
    }
}
