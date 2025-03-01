using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceData
{
    public int id;
    public int groupId;
    public string text;
    public int choiceValue;
    public int startIndex;
    public int endIndex;

    public ChoiceData(string[] data)
    {
        id = string.IsNullOrWhiteSpace(data[0]) ? -1 : Convert.ToInt16(data[0]);
        groupId = string.IsNullOrWhiteSpace(data[1]) ? -1 : Convert.ToInt16(data[1]);
        text = string.IsNullOrWhiteSpace(data[2]) ? "" : data[2];
        choiceValue = string.IsNullOrWhiteSpace(data[3]) ? -1 : Convert.ToInt16(data[3]);
        startIndex = string.IsNullOrWhiteSpace(data[4]) ? -1 : Convert.ToInt16(data[4]);
        endIndex = string.IsNullOrWhiteSpace(data[5]) ? -1 : Convert.ToInt16(data[5]);
    }
}

public class ChoiceCSVReader : MonoBehaviour
{
    private int _dataLength;

    public ChoiceData[] LoadData(CSVLoader csvLoader, string filePath)
    {
        List<string[]> choiceCSVData = csvLoader.LoadCSV(filePath);

        List<ChoiceData> choiceData = new List<ChoiceData>();

        for (int index = 0; index < choiceCSVData.Count; index++)
        {
            if (index == 0)
            {
                _dataLength = choiceCSVData[index].Length;
                continue;
            }

            string[] rawData = choiceCSVData[index];

            if (rawData.Length != _dataLength)
            {
                Debug.LogError($"{index}번째 Choice 데이터 에러");
            }

            ChoiceData choiceDatum = new ChoiceData(rawData);

            choiceData.Add(choiceDatum);
        }

        return choiceData.ToArray();
    }
}
