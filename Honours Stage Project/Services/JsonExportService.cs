using Honours_Stage_Project.ViewModels;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Honours_Stage_Project.Services
{
    public class JsonExportService : IExportService
    {
        private readonly IFileService _fileService;
        private readonly IImportService _importService;

        public JsonExportService(IFileService fileService, IImportService importService)
        {
            _fileService = fileService;
            _importService = importService;
        }

        public void Export(IEnumerable<object> nodeExports, IEnumerable<Connection> connections, string fileName = "exported_data")
        {
            string path = GetSaveFilePath("lastExportLocation.txt", fileName);
            if (string.IsNullOrWhiteSpace(path))
                return;

            var textBoxData = nodeExports.ToList();

            var connectionObjects = connections.Select(c => (object)new
            {
                FromTextBoxID = c.NodeId,
                FromComponentID = c.ComponentId,
                FromConnectionID = c.ConnectionId,
                ToTextBoxID = c.TargetNodeId
            }).ToList();

            var dataPackage = new
            {
                TextBoxes = textBoxData,
                Connections = connectionObjects,
                LuaStubs = _importService.CurrentLuaStub
            };

            string json = JsonConvert.SerializeObject(dataPackage, Formatting.Indented);
            _fileService.WriteAllText(path, json);
        }

        private static string GetSaveFilePath(string savedPathLocation, string defaultFileName)
        {
            string initialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (File.Exists(savedPathLocation))
            {
                string lastDirectory = File.ReadAllText(savedPathLocation);
                if (!string.IsNullOrWhiteSpace(lastDirectory) && Directory.Exists(lastDirectory))
                    initialDirectory = lastDirectory;
            }

            var dialog = new SaveFileDialog
            {
                Title = "Save dialogue export",
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                DefaultExt = ".json",
                AddExtension = true,
                OverwritePrompt = true,
                InitialDirectory = initialDirectory,
                FileName = string.IsNullOrWhiteSpace(defaultFileName)
                    ? "exported_data"
                    : Path.GetFileNameWithoutExtension(defaultFileName)
            };

            bool? result = dialog.ShowDialog();
            if (result != true)
                return null;

            string selectedDirectory = Path.GetDirectoryName(dialog.FileName);
            if (!string.IsNullOrWhiteSpace(selectedDirectory))
                File.WriteAllText(savedPathLocation, selectedDirectory);

            return dialog.FileName;
        }
    }
}
