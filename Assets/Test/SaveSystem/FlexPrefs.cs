using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class FlexPrefs
{
    private static ISaveSystem _saveSystem;
    private static int _userId = 0;  // Default userId. Change as needed.
    private static Dictionary<string, object> _cache = new Dictionary<string, object>();
    private static bool _isInitialized = false;
    private static readonly string fileName = "FlexPrefsData";

    /// <summary>
    /// Initializes FlexPrefs with the specified save system and user ID.
    /// This must be called before using any other methods in FlexPrefs.
    /// </summary>
    /// <param name="saveSystem">The save system implementation to use for saving and loading data.</param>
    /// <param name="userId">The user ID for loading user-specific data. Defaults to 0.</param>
    public static async Task Initialize(ISaveSystem saveSystem, int userId = 0)
    {
        _saveSystem = saveSystem;
        _userId = userId;
        _cache = await LoadData();
        _isInitialized = true;
    }

    /// <summary>
    /// Loads the saved data into the cache asynchronously.
    /// </summary>
    /// <returns>A dictionary containing the saved key-value pairs, or an empty dictionary if no data is found.</returns>
    private static async Task<Dictionary<string, object>> LoadData()
    {
        var data = await _saveSystem.Load<Dictionary<string, object>>(_userId, fileName);
        return data ?? new Dictionary<string, object>();
    }

    /// <summary>
    /// Gets the value associated with the specified key.
    /// If the key does not exist, the default value is returned.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The key associated with the value.</param>
    /// <param name="defaultValue">The default value to return if the key does not exist. Optional.</param>
    /// <returns>The value of type T if the key exists, otherwise the default value.</returns>
    public static T Get<T>(string key, T defaultValue = default)
    {
        if (!_isInitialized)
        {
            Debug.LogError("FlexPrefs not initialized. Call FlexPrefs.Initialize() first.");
            return defaultValue;
        }

        if (_cache.TryGetValue(key, out var value) && value is T)
        {
            return (T)value;
        }
        return defaultValue;
    }

    /// <summary>
    /// Sets the value for the specified key.
    /// If the key already exists, the value is updated.
    /// </summary>
    /// <typeparam name="T">The type of the value to set.</typeparam>
    /// <param name="key">The key to associate with the value.</param>
    /// <param name="value">The value to store.</param>
    public static void Set<T>(string key, T value)
    {
        if (!_isInitialized)
        {
            Debug.LogError("FlexPrefs not initialized. Call FlexPrefs.Initialize() first.");
            return;
        }

        if (_cache.ContainsKey(key))
        {
            _cache[key] = value;
        }
        else
        {
            _cache.Add(key, value);
        }
    }

    /// <summary>
    /// Saves the current state of the cache to the persistent storage asynchronously.
    /// </summary>
    public static async void Save()
    {
        if (!_isInitialized)
        {
            Debug.LogError("FlexPrefs not initialized. Call FlexPrefs.Initialize() first.");
            return;
        }

        await _saveSystem.Save(_userId, fileName, _cache);
    }

    /// <summary>
    /// Checks if a specific key exists in the cache.
    /// </summary>
    /// <param name="key">The key to check for existence.</param>
    /// <returns>True if the key exists, otherwise false.</returns>
    public static bool HasKey(string key)
    {
        if (!_isInitialized)
        {
            Debug.LogError("FlexPrefs not initialized. Call FlexPrefs.Initialize() first.");
            return false;
        }

        return _cache.ContainsKey(key);
    }

    /// <summary>
    /// Clears the value associated with the specified key.
    /// If the key exists, it is removed from the cache and the change is saved.
    /// </summary>
    /// <param name="key">The key to clear.</param>
    public static async void ClearKey(string key)
    {
        if (!_isInitialized)
        {
            Debug.LogError("FlexPrefs not initialized. Call FlexPrefs.Initialize() first.");
            return;
        }

        if (_cache.ContainsKey(key))
        {
            _cache.Remove(key);
            await _saveSystem.Save(_userId, fileName, _cache);
        }
    }

    /// <summary>
    /// Clears all keys and values from the cache.
    /// This also saves the cleared state to persistent storage.
    /// </summary>
    public static async void Clear()
    {
        if (!_isInitialized)
        {
            Debug.LogError("FlexPrefs not initialized. Call FlexPrefs.Initialize() first.");
            return;
        }

        _cache.Clear();
        await _saveSystem.Save(_userId, fileName, _cache);
    }
}
