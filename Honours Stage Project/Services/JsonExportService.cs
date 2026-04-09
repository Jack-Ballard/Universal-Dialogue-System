using Honours_Stage_Project.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Honours_Stage_Project.Services
{
    public class JsonExportService : IExportService
    {
        private readonly IFileService _fileService;

        public JsonExportService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public void Export(IEnumerable<NodeViewModel> nodes, IEnumerable<Connection> connections, string fileName = "exported_data")
        {
            var textBoxData = nodes.Select(n => n.Model.Export()).ToList();

            var connectionObjects = connections.Select(c => (object)new
            {
                FromTextBoxID = c.NodeId,
                FromComponentID = c.ComponentId,
                FromConnectionID = c.ConnectionId,
                ToTextBoxID = c.TargetNodeId           
            }).ToList();

            var dataPackage = new { TextBoxes = textBoxData, Connections = connectionObjects };
            string json = JsonConvert.SerializeObject(dataPackage, Formatting.Indented);
            _fileService.WriteAllText(fileName+".json", json);
        }
    }
}
