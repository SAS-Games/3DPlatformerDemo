using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SAS.Utilities.TagSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Debug = SAS.Debug;

public class JsonFileSaveSystem : ISaveSystem
{
    private readonly string rootDir = Application.persistentDataPath;

    public JsonFileSaveSystem(IContextBinder _)
    {

    }

    async Task<T> ISaveSystem.Load<T>(int userId, string fileName)
    {
        return await ((ISaveSystem)this).Load<T>(userId, "Saves", fileName);
    }

    async Task<T> ISaveSystem.Load<T>(int userId, string dirName, string fileName)
    {
        var filePath = Path.Combine(rootDir, dirName, userId.ToString(), fileName + ".json");

        if (File.Exists(filePath))
        {
            string json = string.Empty;
            try
            {
                json = await File.ReadAllTextAsync(filePath);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            if (!string.IsNullOrWhiteSpace(json))
            {
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                };

                T data = JsonConvert.DeserializeObject<T>(json, settings);
                if (data != null)
                    return data;
            }
        }

        Debug.LogWarning($"File not found or empty: {filePath}");
        return new T(); // This now works because of the where T : new() constraint
    }

    async Task ISaveSystem.Save<T>(int userId, string fileName, T data)
    {
        await ((ISaveSystem)this).Save(userId, "Saves", fileName, data);
    }

    async Task ISaveSystem.Save<T>(int userId, string dirName, string fileName, T data)
    {
        try
        {
            var directoryPath = Path.Combine(rootDir, dirName, userId.ToString());
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            string filePath = Path.Combine(directoryPath, fileName + ".json");
            // Include type information when serializing
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };
            string json = JsonConvert.SerializeObject(data, settings);

            await File.WriteAllTextAsync(filePath, json);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save file: {ex.Message}");
        }
    }

    void IBindable.OnInstanceCreated()
    {
    }
}
