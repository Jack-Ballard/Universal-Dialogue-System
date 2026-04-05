using System.Collections.Generic;
using System.Linq;
using Honours_Stage_Project.ViewModels;
using Newtonsoft.Json;

namespace Honours_Stage_Project.Services
{
    public class JsonExportService : IExportService
    {
        private const string OutputPath = "exported_data.json";

        private readonly IFileService _fileService;

        public JsonExportService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public void Export(IEnumerable<NodeViewModel> nodes, IEnumerable<(int, int, int)> connections)
        {
            var textBoxData = nodes.Select(n => n.Model.Export()).Cast<object>().ToList();

            var connectionObjects = connections.Select(c => (object)new
            {
                FromTextBoxID = c.Item1,
                FromComponentID = c.Item2,
                ToTextBoxID = c.Item3
            }).ToList();

            var dataPackage = new { TextBoxes = textBoxData, Connections = connectionObjects };
            string json = JsonConvert.SerializeObject(dataPackage, Formatting.Indented);
            _fileService.WriteAllText(OutputPath, json);
        }
    }
}
