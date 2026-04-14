using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Honours_Stage_Project.Models;
using Honours_Stage_Project.ViewModels;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Honours_Stage_Project.Services
{
    public class JsonImportService : IImportService
    {
        private readonly IFileService _fileService;

        public JsonImportService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public (List<NodeViewModel>, List<Connection>) ImportDialogue(INodeConnectionService connectionService)
        {
            string path = GetFilePath("lastFileLocation.txt");

            if (string.IsNullOrEmpty(path))
                return (new List<NodeViewModel>(), new List<Connection>());

            _fileService.ReadAllText(path, out string json);

            var root = JsonConvert.DeserializeObject<JObject>(json);
            if (root == null)
                return (new List<NodeViewModel>(), new List<Connection>());

            var nodes = new List<NodeViewModel>();
            var nodeTokens = root["TextBoxes"] as JArray ?? new JArray();

            foreach (var nodeToken in nodeTokens.OfType<JObject>())
            {
                var nodeId = GetInt(nodeToken, "ID");
                var model = new NodeModel(nodeId);
                model.Import(nodeToken);

                nodes.Add(new NodeViewModel(model, connectionService));
            }

            var connections = new List<Connection>();
            var connectionTokens = root["Connections"] as JArray ?? new JArray();
            foreach (var connectionToken in connectionTokens.OfType<JObject>())
            {
                Connection connection = new Connection(
                    GetInt(connectionToken, "FromTextBoxID"),
                    GetInt(connectionToken, "FromComponentID"),
                    GetInt(connectionToken, "FromConnectionID"),
                    GetInt(connectionToken, "ToTextBoxID"));

                connections.Add(connection);
            }

            return (nodes, connections);
        }

        public LuaStubDefinition ImportLuaStub()
        {
            string path = GetFilePath("lastStubLocation.txt");

            _fileService.ReadAllText(path, out string json);

            if (string.IsNullOrWhiteSpace(json))
                return new LuaStubDefinition();

            var root = JsonConvert.DeserializeObject<JObject>(json);
            if (root == null)
                return new LuaStubDefinition();

            var result = new LuaStubDefinition();

            var variables = root["variables"] as JObject;
            if (variables != null)
            {
                foreach (var property in variables.Properties())
                {
                    var variableObject = property.Value as JObject;
                    if (variableObject == null)
                        continue;

                    result.Variables[property.Name] = new LuaStubVariable
                    {
                        Type = GetString(variableObject, "type"),
                        Value = variableObject["value"] == null ? null : variableObject["value"].ToObject<object>()
                    };
                }
            }

            result.Functions = ParseMembers(root["functions"] as JArray);
            result.Attributes = ParseMembers(root["attributes"] as JArray);

            return result;
        }

        private static List<LuaStubMember> ParseMembers(JArray members)
        {
            var result = new List<LuaStubMember>();
            if (members == null)
                return result;

            foreach (var token in members.OfType<JObject>())
            {
                var member = new LuaStubMember
                {
                    Name = GetString(token, "name"),
                    DeclaringType = GetString(token, "declaringType"),
                    ReturnType = GetString(token, "returnType"),
                    Parameters = ParseParameters(token["parameters"] as JArray)
                };

                if (!string.IsNullOrWhiteSpace(member.Name))
                    result.Add(member);
            }

            return result;
        }

        private static List<LuaStubParameter> ParseParameters(JArray parameters)
        {
            var result = new List<LuaStubParameter>();
            if (parameters == null)
                return result;

            foreach (var token in parameters.OfType<JObject>())
            {
                result.Add(new LuaStubParameter
                {
                    Name = GetString(token, "name"),
                    Type = GetString(token, "type")
                });
            }

            return result;
        }

        private static string GetFilePath(string savedPathLocation)
        {
            string InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (File.Exists(savedPathLocation))
            {
                InitialDirectory = File.ReadAllText(savedPathLocation);
            }
            var dialog = new OpenFileDialog
            {
                Title = "Select a file",
                Filter = "All files (*.*)|*.*",
                InitialDirectory = InitialDirectory
            };

            bool? result = dialog.ShowDialog();

            if (result == false)
            {
                return null;
            }

            string path = dialog.FileName;

            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            File.WriteAllText(savedPathLocation, dialog.FileName);

            return path;
        }

        private static int GetInt(JObject token, string propertyName)
            => (int?)token[propertyName] ?? 0;

        private static string GetString(JObject token, string propertyName)
            => (string)token[propertyName] ?? string.Empty;
    }
}
