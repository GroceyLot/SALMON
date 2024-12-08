using UnityEngine;
using System.IO;

[System.Serializable]
public class Saveable
{
    public int checkpoint = 0;
}

public class SaveData : MonoBehaviour
{
    public string filename;
    private string filePath;
    public Saveable data;

    void Start()
    {
        filePath = Application.persistentDataPath + $"/{filename}.json";
        Debug.Log(filePath);
        if (File.Exists(filePath))
        {
            string jsonString = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<Saveable>(jsonString);
        }
        else
        {
            data = new Saveable();
            string jsonString = JsonUtility.ToJson(data);
            File.WriteAllText(filePath, jsonString);
        }
    }

    public void Save()
    {
        string jsonString = JsonUtility.ToJson(data);
        File.WriteAllText(filePath, jsonString);
    }
}