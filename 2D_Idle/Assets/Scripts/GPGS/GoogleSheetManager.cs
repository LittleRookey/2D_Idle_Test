using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class GoogleSheetManager : MonoBehaviour
{
    const string URL = "https://sheets.googleapis.com/v4/spreadsheets/YOUR_SPREADSHEET_ID/values/Sheet1!A1:Z1000?key=YOUR_API_KEY";

    private Dictionary<string, Dictionary<string, string>> stats = new Dictionary<string, Dictionary<string, string>>();

    IEnumerator Start()
    {
        UnityWebRequest www = UnityWebRequest.Get(URL);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string data = www.downloadHandler.text;
            ParseData(data);
        }
        else
        {
            Debug.LogError("Error: " + www.error);
        }
    }

    void ParseData(string data)
    {
        JSONNode root = JSON.Parse(data);
        JSONArray values = root["values"].AsArray;

        // Assuming the first row contains the keys
        JSONNode keys = values[0];

        for (int i = 1; i < values.Count; i++)
        {
            JSONNode row = values[i];
            Dictionary<string, string> rowData = new Dictionary<string, string>();

            for (int j = 0; j < row.Count; j++)
            {
                rowData[keys[j]] = row[j];
            }

            stats[rowData["Key"]] = rowData;
        }

        // Now you can access the data like this:
        foreach (var stat in stats)
        {
            string key = stat.Key;
            Dictionary<string, string> m_values = stat.Value;

            Debug.Log("Key: " + key);
            foreach (var value in m_values)
            {
                Debug.Log(value.Key + ": " + value.Value);
            }
        }
    }
}