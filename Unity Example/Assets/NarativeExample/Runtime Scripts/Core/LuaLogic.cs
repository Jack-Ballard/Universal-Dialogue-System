// Each tuple: (segment, isInsideBraces)
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MoonSharp.Interpreter;

public static class LuaLogic
{
    public static string FormatByLua(string input)
    {
        Script luaScript = new Script(CoreModules.Preset_Default);

        SyncVariables(luaScript);

        var result = new List<(string segment, bool isInside)>();
        var braceRegex = new Regex(@"\{(.*?)\}", RegexOptions.Singleline);
        var dollarRegex = new Regex(@"\$([a-zA-Z_][a-zA-Z0-9_]*)");

        string output = "";
        result = SplitByRegex(input, braceRegex);
        output = EvaluateLua(luaScript, result, false);
        result = SplitByRegex(output, dollarRegex);
        output = EvaluateLua(luaScript, result, true);

        return output;
    }

    public static string EvaluateLua(Script luaScript, List<(string segment, bool isInside)> result, bool useReturn)
    {
        string output = "";

        for (int i = 0; i < result.Count; i++)
        {
            if (result[i].isInside)
            {
                //result[i] = (Regex.Replace(result[i].segment, @"[\r\n\t]", " "), result[i].isInside);
                string luaCode = useReturn ? "return " + result[i].segment.Trim().TrimStart('$') : result[i].segment.Trim();
                if (useReturn && Globals.functions.ContainsKey(result[i].segment) && !result[i].segment.EndsWith("()"))
                {
                    luaCode += "()";
                }
                var luaResult = luaScript.DoString(luaCode).ToString();
                if (luaResult.Length >= 2 && luaResult.StartsWith("\"") && luaResult.EndsWith("\""))
                {
                    luaResult = luaResult.Substring(1, luaResult.Length - 2);
                }
                result[i] = (luaResult, true);
            }
            output += result[i].segment;
        }

        return output;
    }

    public static bool EvaluateLuaCondition(string input)
    {
        Script luaScript = new Script(CoreModules.Preset_Default);
        SyncVariables(luaScript);
        var result = luaScript.DoString("return " + input).ToObject();
        return result is bool boolResult && boolResult;
    }

    public static List<(string segment, bool isInside)> SplitByRegex(string input, Regex regex)
    {
        var result = new List<(string segment, bool isInside)>();
        int lastIndex = 0;

        foreach (Match match in regex.Matches(input))
        {
            // Add text outside the current braces
            if (match.Index > lastIndex)
            {
                result.Add((input.Substring(lastIndex, match.Index - lastIndex), false));
            }
            // Add text inside the braces
            result.Add((match.Groups[1].Value, true));
            lastIndex = match.Index + match.Length;
        }

        // Add any remaining text after the last brace
        if (lastIndex < input.Length)
        {
            result.Add((input.Substring(lastIndex), false));
        }

        return result;
    }

    private static void SyncVariables(Script luaScript)
    {
        foreach (var variable in Globals.variables)
        {
            luaScript.Globals[variable.Key] = Globals.variables[variable.Key];
        }

        foreach (var function in Globals.functions)
        {
            luaScript.Globals[function.Key] = function.Value;
        }
    }
}