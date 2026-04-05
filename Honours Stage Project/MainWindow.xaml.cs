using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Honours_Stage_Project.Node;
using Honours_Stage_Project.ViewModels;

namespace Honours_Stage_Project
{
    public partial class MainWindow : Window
    {
        private readonly List<Line> _lines = new List<Line>();

        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.RequestLinesRefresh += RefreshLines;
        }

        // ── Line drawing ─────────────────────────────────────────────────────
        // Line coordinates are calculated from NodeViewModel.X/Y and fixed layout
        // constants defined in MainWindowViewModel.  This is intentionally kept in
        // the code-behind because it is purely presentational (creating WPF shapes).

        private void RefreshLines()
        {
            var vm = (MainWindowViewModel)DataContext;

            foreach (var line in _lines)
                LinesCanvas.Children.Remove(line);
            _lines.Clear();

            foreach (var (fromNodeId, fromComponentId, toNodeId) in vm.Connections)
            {
                var sourceNode = vm.Nodes.FirstOrDefault(n => n.Model.ID == fromNodeId);
                var targetNode = vm.Nodes.FirstOrDefault(n => n.Model.ID == toNodeId);
                if (sourceNode == null || targetNode == null) continue;

                int componentIndex = -1;
                for (int i = 0; i < sourceNode.ConnectionComponents.Count; i++)
                {
                    if (sourceNode.ConnectionComponents[i].ID == fromComponentId)
                    {
                        componentIndex = i;
                        break;
                    }
                }

                double y1Offset = componentIndex >= 0
                    ? componentIndex * MainWindowViewModel.ComponentSpacing
                    : 0;

                var newLine = new Line
                {
                    Stroke          = Brushes.LightSteelBlue,
                    StrokeThickness = 2,
                    X1 = sourceNode.X + MainWindowViewModel.NodeWidth,
                    Y1 = sourceNode.Y + MainWindowViewModel.HeaderHeight + y1Offset,
                    X2 = targetNode.X,
                    Y2 = targetNode.Y + MainWindowViewModel.HeaderHeight / 2,
                };

                _lines.Add(newLine);
                LinesCanvas.Children.Add(newLine);
            }
        }
    }
}

