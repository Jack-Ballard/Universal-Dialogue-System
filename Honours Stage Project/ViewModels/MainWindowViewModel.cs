using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Honours_Stage_Project.Helpers;
using Honours_Stage_Project.Models;
using Honours_Stage_Project.Services;

namespace Honours_Stage_Project.ViewModels
{
    public class MainWindowViewModel
    {
        // Layout constants used by the view to draw connection lines.
        public const double NodeWidth = 281;
        public const double HeaderHeight = 50;
        public const double ComponentSpacing = 80;

        private readonly INodeConnectionService _connectionService;
        private readonly IExportService _exportService;
        private readonly IImportService _importService;
        private readonly ILuaStubValidationService _luaStubValidationService;

        public ObservableCollection<NodeViewModel> Nodes { get; } = new ObservableCollection<NodeViewModel>();

        public IReadOnlyList<Connection> Connections => _connectionService.Connections;

        public ICommand AddNodeCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand ImportCommand { get; }
        public ICommand ImportStubCommand { get; }
        public ICommand DeleteNodeCommand { get; }

        /// <summary>Raised whenever connection lines should be redrawn.</summary>
        public event Action RequestLinesRefresh;

        public MainWindowViewModel(
            INodeConnectionService connectionService,
            IExportService exportService,
            IImportService importService,
            ILuaStubValidationService luaStubValidationService)
        {
            _connectionService = connectionService;
            _exportService = exportService;
            _importService = importService;
            _luaStubValidationService = luaStubValidationService;

            _connectionService.ConnectionsChanged += () => RequestLinesRefresh?.Invoke();

            AddNodeCommand = new RelayCommand(parameter => AddNode(parameter));
            ExportCommand = new RelayCommand(_ => _exportService.Export(Nodes, _connectionService.Connections));
            ImportCommand = new RelayCommand(_ => Import());
            ImportStubCommand = new RelayCommand(_ => ImportStub());
            DeleteNodeCommand = new RelayCommand(node => RemoveNode(node as NodeViewModel));
        }

        public void AddNodeAt(double x, double y)
        {
            var model = new NodeModel(Nodes.Count)
            {
                X = x,
                Y = y
            };

            var viewModel = new NodeViewModel(model, _connectionService, _luaStubValidationService, "lua_api_export.json");
            AttachNode(viewModel);

            Nodes.Add(viewModel);
        }

        private void AddNode(object parameter)
        {
            if (parameter is Point point)
            {
                AddNodeAt(point.X, point.Y);
                return;
            }

            AddNodeAt(50, 50);
        }

        private void RemoveNode(NodeViewModel node)
        {
            DetachNode(node);
            Nodes.Remove(node);
            for (int i = 0; i < Nodes.Count; i++)
                Nodes[i].Model.ID = i;
            _connectionService.RemoveConnectionsForNode(node.Model.ID);
        }

        private void Import()
        {
            var (importedNodes, importedConnections) = _importService.ImportDialogue(_connectionService);

            if(importedNodes == null || importedConnections == null)
                return;

            foreach (var node in Nodes)
                DetachNode(node);

            Nodes.Clear();

            foreach (var node in importedNodes)
            {
                node.ConfigureLuaValidation(_luaStubValidationService);
                AttachNode(node);
                Nodes.Add(node);
            }

            _connectionService.SetConnections(importedConnections);

            // Validate imported node text as Lua against the generated stub.
            ValidateImportedLuaScripts();

            RequestLinesRefresh?.Invoke();
        }

        private void ImportStub()
        {
            _importService.ImportLuaStub();

            foreach (var node in Nodes)
                node.ConfigureLuaValidation(_luaStubValidationService);

            ValidateImportedLuaScripts();
        }

        private void ValidateImportedLuaScripts()
        {
            if (!_luaStubValidationService.CanValidateLua)
                return;

            var allErrors = new List<string>();

            foreach (var node in Nodes)
            {
                string luaScript = node?.Model?.TextContent;
                if (string.IsNullOrWhiteSpace(luaScript))
                    continue;

                List<LuaValidationResult> validation = _luaStubValidationService.ValidateLua(luaScript);

                foreach (LuaValidationResult result in validation)
                {
                    if (result.IsValid)
                        continue;

                    foreach (string error in result.Errors)
                        allErrors.Add("Node " + node.Model.ID + ": " + error);
                }
            }

            if (allErrors.Count == 0)
                return;

            MessageBox.Show(
                string.Join(Environment.NewLine, allErrors),
                "Lua validation errors",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        private void AttachNode(NodeViewModel node)
        {
            node.PropertyChanged -= Node_PropertyChanged;
            node.PropertyChanged += Node_PropertyChanged;
        }

        private void DetachNode(NodeViewModel node)
        {
            node.PropertyChanged -= Node_PropertyChanged;
        }

        private void Node_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(NodeViewModel.X)
                || e.PropertyName == nameof(NodeViewModel.Y)
                || e.PropertyName == nameof(NodeViewModel.Size))
                RequestLinesRefresh?.Invoke();
        }
    }
}
