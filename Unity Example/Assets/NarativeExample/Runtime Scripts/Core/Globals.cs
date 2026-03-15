using System.Collections.Generic;
using System;

public static class Globals
{
    public static List<TextBoxes> textBoxes = new List<TextBoxes>();
    public static List<(int, int, int)> connections = new List<(int, int, int)>();
    public static Dictionary<string, object> variables = new Dictionary<string, object>();
    public static Dictionary<string, Delegate> functions = new Dictionary<string, Delegate>();

    //public static void InitaliseVariables()
    //{
    //    variables["score"] = 42;
    //    variables["playerName"] = "Not Assigned";
    //    variables["health"] = 100;
    //}

    public static void AddFunction(Delegate function)
    {
        functions[function.Method.Name] = function;
    }

    public static void AddVariables(string variableName, object variableValue)
    {
        variables[variableName] = variableValue;
    }
}