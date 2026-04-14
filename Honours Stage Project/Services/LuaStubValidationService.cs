using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Honours_Stage_Project.Services
{
    public class LuaStubValidationService : ILuaStubValidationService
    {
        private readonly IImportService _importService;

        public LuaStubValidationService(IImportService importService)
        {
            _importService = importService;
        }

        public static List<string> FormatByLua(string input)
        {
            Script luaScript = new Script(CoreModules.Preset_Default);

            var result = new List<string>();
            var regex = new Regex(@"\{(.*?)\}", RegexOptions.Singleline);

            int lastIndex = 0;

            foreach (Match match in regex.Matches(input))
            {

                result.Add((match.Groups[1].Value));

                lastIndex = match.Index + match.Length;
            }

            return result;
        }

        public List<LuaValidationResult> ValidateLua(string luaScript)
        {
            List<string> luaSnippets = FormatByLua(luaScript);
            var result = new List<LuaValidationResult>();
            foreach(var luaSnippet in luaSnippets)
            {
                 result.Add(Validate(luaSnippet));
            }
            return result;
        }

        public LuaValidationResult Validate(string luaScript)
        {
            var result = new LuaValidationResult();

            if (string.IsNullOrWhiteSpace(luaScript))
            {
                result.Errors.Add("Lua script is empty.");
                return result;
            }

            LuaStubDefinition stub;
            try
            {
                stub = _importService.ImportLuaStub();
            }
            catch (Exception e)
            {
                result.Errors.Add("Failed to import Lua stubs: " + e.Message);
                return result;
            }

            var script = new Script(CoreModules.Preset_Default);
            RegisterStubs(script, stub);
            GuardUnknownGlobals(script);

            try
            {
                script.LoadString(luaScript);
                script.DoString(luaScript);
            }
            catch (SyntaxErrorException e)
            {
                result.Errors.Add("Syntax error: " + e.DecoratedMessage);
            }
            catch (ScriptRuntimeException e)
            {
                result.Errors.Add("Runtime error: " + e.DecoratedMessage);
            }
            catch (Exception e)
            {
                result.Errors.Add("Unexpected error: " + e.Message);
            }

            return result;
        }

        private static void RegisterStubs(Script script, LuaStubDefinition stub)
        {
            if (stub == null)
                return;

            foreach (var pair in stub.Variables)
            {
                script.Globals[pair.Key] = ToDynValue(pair.Value == null ? null : pair.Value.Value);
            }

            RegisterMembers(script, stub.Functions);
            RegisterMembers(script, stub.Attributes);
        }

        private static void RegisterMembers(Script script, List<LuaStubMember> members)
        {
            if (members == null)
                return;

            foreach (var member in members)
            {
                if (member == null || string.IsNullOrWhiteSpace(member.Name))
                    continue;

                script.Globals[member.Name] = DynValue.NewCallback((context, args) =>
                {
                    ValidateCall(member, args);
                    return DefaultReturn(member.ReturnType);
                });
            }
        }

        private static void ValidateCall(LuaStubMember member, CallbackArguments args)
        {
            int expected = member.Parameters == null ? 0 : member.Parameters.Count;
            int actual = args.Count;

            if (actual != expected)
            {
                throw new ScriptRuntimeException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Function '{0}' expected {1} argument(s), got {2}.",
                        member.Name,
                        expected,
                        actual));
            }

            if (member.Parameters == null)
                return;

            for (int i = 0; i < member.Parameters.Count; i++)
            {
                if (!IsCompatibleType(args[i], member.Parameters[i].Type))
                {
                    throw new ScriptRuntimeException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Function '{0}' argument {1} expected type '{2}'.",
                            member.Name,
                            i + 1,
                            member.Parameters[i].Type));
                }
            }
        }

        private static void GuardUnknownGlobals(Script script)
        {
            var meta = new Table(script);
            meta.Set("__index", DynValue.NewCallback((context, args) =>
            {
                var key = args[1].CastToString();
                var existing = script.Globals.RawGet(key);

                if (existing != null && existing.Type != DataType.Nil)
                    return existing;

                throw new ScriptRuntimeException("Unknown global: " + key);
            }));

            script.Globals.MetaTable = meta;
        }

        private static DynValue ToDynValue(object value)
        {
            if (value == null)
                return DynValue.Nil;

            if (value is bool boolValue)
                return DynValue.NewBoolean(boolValue);

            if (value is string stringValue)
                return DynValue.NewString(stringValue);

            if (value is int intValue)
                return DynValue.NewNumber(intValue);

            if (value is long longValue)
                return DynValue.NewNumber(longValue);

            if (value is float floatValue)
                return DynValue.NewNumber(floatValue);

            if (value is double doubleValue)
                return DynValue.NewNumber(doubleValue);

            if (value is decimal decimalValue)
                return DynValue.NewNumber((double)decimalValue);

            return DynValue.NewString(value.ToString());
        }

        private static DynValue DefaultReturn(string returnType)
        {
            if (string.IsNullOrWhiteSpace(returnType) || returnType == "System.Void")
                return DynValue.Nil;

            if (returnType == "System.Boolean")
                return DynValue.NewBoolean(false);

            if (returnType == "System.String")
                return DynValue.NewString(string.Empty);

            if (returnType == "System.Int16"
                || returnType == "System.Int32"
                || returnType == "System.Int64"
                || returnType == "System.Single"
                || returnType == "System.Double"
                || returnType == "System.Decimal")
                return DynValue.NewNumber(0);

            return DynValue.Nil;
        }

        private static bool IsCompatibleType(DynValue value, string expectedClrType)
        {
            if (string.IsNullOrWhiteSpace(expectedClrType))
                return true;

            switch (expectedClrType)
            {
                case "System.Boolean":
                    return value.Type == DataType.Boolean;
                case "System.String":
                    return value.Type == DataType.String;
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.Single":
                case "System.Double":
                case "System.Decimal":
                    return value.Type == DataType.Number;
                default:
                    return true;
            }
        }
    }
}