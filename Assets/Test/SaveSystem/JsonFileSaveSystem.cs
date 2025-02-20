using SAS.Utilities.TagSystem;
using System.IO;
using UnityEngine;

public class JsonFileSaveSystem : ISaveSystem
{
    private readonly string DirectoryPath;

    public JsonFileSaveSystem(IContextBinder _)
    {
        DirectoryPath = Path.Combine(Application.persistentDataPath, "Saves");
        if (!Directory.Exists(DirectoryPath))
            Directory.CreateDirectory(DirectoryPath);
    }

    void ISaveSystem.Save<T>(string fileName, T data)
    {
        string filePath = Path.Combine(DirectoryPath, fileName + ".json");
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
    }

    T ISaveSystem.Load<T>(string fileName)
    {
        string filePath = Path.Combine(DirectoryPath, fileName + ".json");

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<T>(json);
        }

        Debug.LogWarning($"File not found: {filePath}");
        return default;
    }

    void IBindable.OnInstanceCreated()
    {
    }
}
