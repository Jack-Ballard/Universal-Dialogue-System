using System;
using System.Collections.ObjectModel;
using System.Linq;
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

        public ObservableCollection<NodeViewModel> Nodes { get; } = new ObservableCollection<NodeViewModel>();

        public IReadOnlyList<(int, int, int)> Connections => _connectionService.Connections;

        public ICommand AddNodeCommand { get; }
        public ICommand ExportCommand { get; }

        /// <summary>Raised whenever connection lines should be redrawn.</summary>
        public event Action RequestLinesRefresh;

        public MainWindowViewModel(INodeConnectionService connectionService, IExportService exportService)
        {
            _connectionService = connectionService;
            _exportService = exportService;

            _connectionService.ConnectionsChanged += () => RequestLinesRefresh?.Invoke();

            AddNodeCommand = new RelayCommand(_ => AddNode());
            ExportCommand = new RelayCommand(_ => _exportService.Export(Nodes, _connectionService.Connections));
        }

        private void AddNode()
        {
            var model = new NodeModel(Nodes.Count);
            var viewModel = new NodeViewModel(model, _connectionService);

            // Raise RequestLinesRefresh whenever a node is repositioned.
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(NodeViewModel.X) || e.PropertyName == nameof(NodeViewModel.Y))
                    RequestLinesRefresh?.Invoke();
            };

            Nodes.Add(viewModel);
        }
    }
}
