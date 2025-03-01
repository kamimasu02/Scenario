using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine;


public class CSVLoader : MonoBehaviour
{
    private Regex _csvRegex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

    public List<string[]> LoadCSV(string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Csvs", fileName + ".csv");

        List<string[]> csvData = new List<string[]>();

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                string[] rowData = _csvRegex.Split(line);
                csvData.Add(rowData);
            }
        }
        else
        {
            Debug.LogError("CSV 파일을 찾을 수 없습니다: " + filePath);
        }

        return csvData;
    }
}
