using SAS.Utilities.TagSystem;
using System.IO;
using System.Threading.Tasks;
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


    async Task<T> ISaveSystem.Load<T>(string fileName)
    {
        string filePath = Path.Combine(DirectoryPath, fileName + ".json");

        if (File.Exists(filePath))
        {
            string json = await Task.Run(() => File.ReadAllText(filePath));

            if (!string.IsNullOrWhiteSpace(json))
            {
                T data = JsonUtility.FromJson<T>(json);
                if (data != null)
                    return data;
            }
        }

        Debug.LogWarning($"File not found or empty: {filePath}");
        return new T();
    }


    void IBindable.OnInstanceCreated()
    {
    }
}
