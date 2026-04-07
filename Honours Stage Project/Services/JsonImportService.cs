using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Honours_Stage_Project.Models;
using Honours_Stage_Project.ViewModels;
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

        public (List<NodeViewModel>, List<(int, int, int, int)>) Import(INodeConnectionService connectionService, string fileName = "exported_data")
        {
            var path = fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
                ? fileName
                : fileName + ".json";

            _fileService.ReadAllText(path, out string json);

            var root = JsonConvert.DeserializeObject<JObject>(json);
            if (root == null)
                return (new List<NodeViewModel>(), new List<(int, int, int, int)>());

            var nodes = new List<NodeViewModel>();
            var nodeTokens = root["TextBoxes"] as JArray ?? new JArray();

            foreach (var nodeToken in nodeTokens.OfType<JObject>())
            {
                var nodeId = GetInt(nodeToken, "ID");
                var model = new NodeModel(nodeId);
                model.Import(nodeToken);

                nodes.Add(new NodeViewModel(model, connectionService));
            }

            var connections = new List<(int, int, int, int)>();
            var connectionTokens = root["Connections"] as JArray ?? new JArray();
            foreach (var connectionToken in connectionTokens.OfType<JObject>())
            {
                connections.Add((
                    GetInt(connectionToken, "FromTextBoxID"),
                    GetInt(connectionToken, "FromComponentID"),
                    GetInt(connectionToken, "FromConnectionID"), // defaults to 0 if absent
                    GetInt(connectionToken, "ToTextBoxID")));
            }

            return (nodes, connections);
        }

        private static int GetInt(JObject token, string propertyName)
            => (int?)token[propertyName] ?? 0;
    }
}
