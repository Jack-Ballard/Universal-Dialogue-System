using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class Globals
{
    public static List<TextBox> textBoxes = new List<TextBox>();
    public static List<Connection> connections = new List<Connection>();
    public static Dictionary<string, object> variables = new Dictionary<string, object>();
    public static Dictionary<string, Delegate> functions = new Dictionary<string, Delegate>();

    public static Dictionary<string, Action<NarrativeAttributeContext>> attributes = new Dictionary<string, Action<NarrativeAttributeContext>>();

    public static void AddFunction(Delegate function)
    {
        functions[function.Method.Name] = function;
    }

    public static void AddVariables(string variableName, object variableValue)
    {
        variables[variableName] = variableValue;
    }

    public static void AddAttribute(string attributeName, Action<NarrativeAttributeContext> attributeHandler)
    {
        if (attributes.TryGetValue(attributeName, out Action<NarrativeAttributeContext> existing))
        {
            attributes[attributeName] = existing + attributeHandler;
        }
        else
        {
            attributes[attributeName] = attributeHandler;
        }
    }

    public static void RemoveAttribute(string attributeName, Action<NarrativeAttributeContext> attributeHandler)
    {
        if (!attributes.TryGetValue(attributeName, out Action<NarrativeAttributeContext> existing))
        {
            return;
        }

        existing -= attributeHandler;

        if (existing == null)
        {
            attributes.Remove(attributeName);
        }
        else
        {
            attributes[attributeName] = existing;
        }
    }

    public static bool TryGetAttribute(string attributeName, out Action<NarrativeAttributeContext> attributeHandler)
    {
        return attributes.TryGetValue(attributeName, out attributeHandler);
    }

    public static void Export(string fileName = "lua_api_export.json")
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        JObject payload = new JObject
        {
            ["variables"] = BuildVariablesJson(),
            ["functions"] = BuildDelegateJson(functions),
            ["attributes"] = BuildDelegateJson(attributes.ToDictionary(
                kvp => kvp.Key,
                kvp => (Delegate)kvp.Value
            ))
        };

        string directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(filePath, payload.ToString(Formatting.Indented));
        Debug.Log("Lua API exported to: " + filePath);
    }

    private static JObject BuildVariablesJson()
    {
        JObject result = new JObject();

        foreach (KeyValuePair<string, object> variable in variables)
        {
            object value = variable.Value;
            result[variable.Key] = new JObject
            {
                ["type"] = value?.GetType().FullName ?? "null",
                ["value"] = ToSafeJToken(value)
            };
        }

        return result;
    }

    private static JArray BuildDelegateJson(Dictionary<string, Delegate> source)
    {
        JArray result = new JArray();

        foreach (KeyValuePair<string, Delegate> pair in source)
        {
            Delegate del = pair.Value;
            var method = del.Method;
            var parameters = method.GetParameters();

            JArray parameterArray = new JArray();
            for (int i = 0; i < parameters.Length; i++)
            {
                parameterArray.Add(new JObject
                {
                    ["name"] = parameters[i].Name,
                    ["type"] = parameters[i].ParameterType.FullName
                });
            }

            result.Add(new JObject
            {
                ["name"] = pair.Key,
                ["declaringType"] = method.DeclaringType != null ? method.DeclaringType.FullName : "unknown",
                ["returnType"] = method.ReturnType.FullName,
                ["parameters"] = parameterArray
            });
        }

        return result;
    }

    private static JToken ToSafeJToken(object value)
    {
        if (value == null)
        {
            return JValue.CreateNull();
        }

        try
        {
            return JToken.FromObject(value);
        }
        catch
        {
            return new JObject
            {
                ["nonSerializable"] = true,
                ["stringValue"] = value.ToString()
            };
        }
    }
}