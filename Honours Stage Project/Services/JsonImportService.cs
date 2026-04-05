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

        public (List<NodeViewModel>, List<(int, int, int)>) Import(INodeConnectionService connectionService, string fileName = "exported_data")
        {
            var path = fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
                ? fileName
                : fileName + ".json";

            _fileService.ReadAllText(path, out string json);

            var root = JsonConvert.DeserializeObject<JObject>(json);
            if (root == null)
                return (new List<NodeViewModel>(), new List<(int, int, int)>());

            var nodes = new List<NodeViewModel>();
            var nodeTokens = root["TextBoxes"] as JArray ?? new JArray();

            foreach (var nodeToken in nodeTokens.OfType<JObject>())
            {
                var nodeId = GetInt(nodeToken, "ID");
                var model = new NodeModel(nodeId)
                {
                    TextContent = (string)nodeToken["TextContent"] ?? string.Empty
                };

                ParsePosition(nodeToken["position"], out var x, out var y);
                model.X = x;
                model.Y = y;

                var componentTokens = nodeToken["Connections"] as JArray ?? new JArray();
                foreach (var componentToken in componentTokens.OfType<JObject>())
                {
                    var connectionModel = new ConnectionModel(GetInt(componentToken, "ID"));

                    var attributeTokens = componentToken["Attributes"] as JArray ?? new JArray();
                    foreach (var attributeToken in attributeTokens.OfType<JObject>())
                    {
                        connectionModel.Attributes.Add(new AttributeModel
                        {
                            Id = ReadId(attributeToken),
                            Name = (string)attributeToken["Name"] ?? string.Empty,
                            Value = (string)attributeToken["Value"] ?? string.Empty
                        });
                    }

                    var conditionTokens = componentToken["Conditions"] as JArray ?? new JArray();
                    foreach (var conditionToken in conditionTokens.OfType<JObject>())
                    {
                        connectionModel.Conditions.Add(new ConditionModel
                        {
                            Id = ReadId(conditionToken),
                            Value = (string)conditionToken["Value"] ?? string.Empty
                        });
                    }

                    model.ConnectionComponents.Add(connectionModel);
                }

                nodes.Add(new NodeViewModel(model, connectionService));
            }

            var connections = new List<(int, int, int)>();
            var connectionTokens = root["Connections"] as JArray ?? new JArray();
            foreach (var connectionToken in connectionTokens.OfType<JObject>())
            {
                connections.Add((
                    GetInt(connectionToken, "FromTextBoxID"),
                    GetInt(connectionToken, "FromComponentID"),
                    GetInt(connectionToken, "ToTextBoxID")));
            }

            return (nodes, connections);
        }

        private static int GetInt(JObject token, string propertyName)
            => (int?)token[propertyName] ?? 0;

        private static int ReadId(JObject token)
            => (int?)(token["ID"] ?? token["Id"]) ?? 0;

        private static void ParsePosition(JToken positionToken, out double x, out double y)
        {
            x = 50;
            y = 50;

            if (positionToken == null)
                return;

            if (positionToken.Type == JTokenType.String)
            {
                var parts = positionToken.ToString().Split(',');
                if (parts.Length == 2)
                {
                    if (TryParseDouble(parts[0], out var parsedX))
                        x = parsedX;
                    if (TryParseDouble(parts[1], out var parsedY))
                        y = parsedY;
                }

                return;
            }

            if (positionToken.Type == JTokenType.Object)
            {
                x = (double?)positionToken["X"] ?? x;
                y = (double?)positionToken["Y"] ?? y;
            }
        }

        private static bool TryParseDouble(string input, out double value)
        {
            return double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                   || double.TryParse(input, NumberStyles.Float, CultureInfo.CurrentCulture, out value);
        }
    }
}
