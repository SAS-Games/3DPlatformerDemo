using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class JsonKeyAttribute : Attribute
{
    public string[] Keys { get; }

    public JsonKeyAttribute(params string[] keys)
    {
        Keys = keys;
    }
}

public static class JTokenExtensions
{
    private static readonly Dictionary<string, Type> _keyTypeCache = new Dictionary<string, Type>();

    static JTokenExtensions()
    {
        // Initialize cache at startup
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.GetCustomAttribute<JsonKeyAttribute>() != null);

        foreach (var type in types)
        {
            var keys = type.GetCustomAttribute<JsonKeyAttribute>().Keys;
            foreach (var key in keys)
            {
                if (_keyTypeCache.ContainsKey(key))
                {
                    throw new InvalidOperationException(
                        $"Root key '{key}' is ambiguous between {_keyTypeCache[key]} and {type}");
                }
                _keyTypeCache[key] = type;
            }
        }
    }

    public static object ToObjectRecursive(this JToken token)
    {
        switch (token.Type)
        {
            case JTokenType.Object:
                var jObject = (JObject)token;
                var dict = new Dictionary<string, object>();

                foreach (var prop in jObject.Properties())
                {
                    // Dynamically find the type by reflecting on all assemblies
                    var customType = InferCustomType(prop.Name);

                    if (customType != null)
                        dict[prop.Name] = prop.Value.ToObject(customType);
                    else
                        dict[prop.Name] = ToObjectRecursive(prop.Value);
                }

                return dict;

            case JTokenType.Array:
                return token.Select(ToObjectRecursive).ToList();

            case JTokenType.Integer:
                return token.Value<long>();  // Use long to cover both int and long

            case JTokenType.Float:
                return token.Value<double>();  // Use double for consistent float handling

            case JTokenType.String:
                return token.Value<string>();

            case JTokenType.Boolean:
                return token.Value<bool>();

            case JTokenType.Null:
                return null;

            default:
                return token.ToString();
        }
    }

    private static Type InferCustomType(string key)
    {
        _keyTypeCache.TryGetValue(key, out var type);
        return type;
    }

}
