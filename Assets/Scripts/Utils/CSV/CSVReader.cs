using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVReader : MonoBehaviour
{
    private int _dataLength;

    public T[] LoadData<T>(CSVLoader csvLoader, string filePath) where T: BaseData
    {
        List<string[]> csvData = csvLoader.LoadCSV(filePath);

        List<T> returnData = new List<T>();

        for (int index = 0; index < csvData.Count; index++)
        {
            if (index == 0)
            {
                _dataLength = csvData[index].Length;
                continue;
            }

            string[] rawData = csvData[index];

            if (rawData.Length != _dataLength)
            {
                Debug.LogError($"{index}번째 데이터 에러");
            }

            T datum = (T)Activator.CreateInstance(typeof(T), (object)rawData);

            returnData.Add(datum);
        }

        return returnData.ToArray();
    }
}