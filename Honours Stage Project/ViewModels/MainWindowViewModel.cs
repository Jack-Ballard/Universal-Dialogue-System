using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
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

        public ObservableCollection<NodeViewModel> Nodes { get; } = new ObservableCollection<NodeViewModel>();

        public IReadOnlyList<(int, int, int)> Connections => _connectionService.Connections;

        public ICommand AddNodeCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand ImportCommand { get; }

        /// <summary>Raised whenever connection lines should be redrawn.</summary>
        public event Action RequestLinesRefresh;

        public MainWindowViewModel(INodeConnectionService connectionService, IExportService exportService, IImportService importService)
        {
            _connectionService = connectionService;
            _exportService = exportService;
            _importService = importService;

            _connectionService.ConnectionsChanged += () => RequestLinesRefresh?.Invoke();

            AddNodeCommand = new RelayCommand(parameter => AddNode(parameter));
            ExportCommand = new RelayCommand(_ => _exportService.Export(Nodes, _connectionService.Connections));
            ImportCommand = new RelayCommand(_ => Import());
        }

        public void AddNodeAt(double x, double y)
        {
            var model = new NodeModel(Nodes.Count)
            {
                X = x,
                Y = y
            };

            var viewModel = new NodeViewModel(model, _connectionService);

            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(NodeViewModel.X) || e.PropertyName == nameof(NodeViewModel.Y))
                    RequestLinesRefresh?.Invoke();
            };

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

        private void Import()
        {
            var (importedNodes, importedConnections) = _importService.Import(_connectionService, "exported_data");
            Nodes.Clear();
            foreach (var node in importedNodes)
                Nodes.Add(node);
            _connectionService.SetConnections(importedConnections);
        }
    }
}
