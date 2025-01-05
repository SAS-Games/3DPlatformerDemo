using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public static class SceneUtility
{
    /// <summary>
    /// Finds a single component of a specified type in a given scene.
    /// </summary>
    /// <typeparam name="T">The type of component to search for.</typeparam>
    /// <param name="sceneName">The name of the scene to search in.</param>
    /// <param name="objectName">Optional: The name of the object to match.</param>
    /// <returns>The found component of type T, or null if not found.</returns>
    public static T FindComponentInScene<T>(string sceneName, string objectName = null) where T : Component
    {
        // Get the target scene
        Scene targetScene = SceneManager.GetSceneByName(sceneName);

        // Ensure the scene is valid and loaded
        if (!targetScene.IsValid() || !targetScene.isLoaded)
        {
            Debug.LogWarning($"Scene {sceneName} is not loaded or is invalid.");
            return null;
        }

        // Get all root objects in the scene
        GameObject[] rootObjects = targetScene.GetRootGameObjects();

        // Search through all root objects
        foreach (GameObject rootObject in rootObjects)
        {
            // Check for the specific component
            T component = rootObject.GetComponentInChildren<T>(true);
            if (component != null)
            {
                // If an object name is provided, ensure it matches
                if (string.IsNullOrEmpty(objectName) || rootObject.name == objectName)
                {
                    Debug.Log($"Found {typeof(T).Name} in scene {sceneName} on object {rootObject.name}.");
                    return component;
                }
            }
        }

        Debug.LogWarning($"Component of type {typeof(T).Name} not found in scene {sceneName}.");
        return null;
    }

    /// <summary>
    /// Finds all components of a specified type in a given scene.
    /// </summary>
    /// <typeparam name="T">The type of component to search for.</typeparam>
    /// <param name="sceneName">The name of the scene to search in.</param>
    /// <returns>A list of components of type T found in the scene.</returns>
    public static List<T> FindComponentsInScene<T>(string sceneName) where T : Component
    {
        List<T> components = new List<T>();

        // Get the target scene
        Scene targetScene = SceneManager.GetSceneByName(sceneName);

        // Ensure the scene is valid and loaded
        if (!targetScene.IsValid() || !targetScene.isLoaded)
        {
            Debug.LogWarning($"Scene {sceneName} is not loaded or is invalid.");
            return components;
        }

        // Get all root objects in the scene
        GameObject[] rootObjects = targetScene.GetRootGameObjects();

        // Search through all root objects
        foreach (GameObject rootObject in rootObjects)
        {
            // Add all components of type T found in the hierarchy
            T[] foundComponents = rootObject.GetComponentsInChildren<T>(true);
            if (foundComponents.Length > 0)
            {
                components.AddRange(foundComponents);
            }
        }

        Debug.Log($"Found {components.Count} components of type {typeof(T).Name} in scene {sceneName}.");
        return components;
    }

    public static void SetActiveScene(Scene scene)
    {
        if (scene.IsValid())
            SceneManager.SetActiveScene(scene);
        else
            Debug.LogWarning($"Active scene {scene.name} not found or is not valid.");
    }
    public static void SetActiveScene(string sceneName)
    {
        var scene = SceneManager.GetSceneByName(sceneName);
        SetActiveScene(scene);
    }
}