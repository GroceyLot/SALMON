using UnityEngine;
using System.IO;

[System.Serializable]
public class Saveable
{
    public string name;
}

public class SaveData : MonoBehaviour
{
    public string filename;
    private string filePath;
    public Saveable data;

    void Start()
    {
        filePath = Application.persistentDataPath + $"/{filename}.json";
        if (File.Exists(filePath))
        {
            string jsonString = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<Saveable>(jsonString);
        }
    }

    public void Save()
    {
        string jsonString = JsonUtility.ToJson(data);
        File.WriteAllText(filePath, jsonString);
    }
}