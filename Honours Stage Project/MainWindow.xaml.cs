using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
            viewModel.RequestLinesRefresh += () =>
            {
                // Ensure UI layout has completed before calculating line coordinates
                Dispatcher.InvokeAsync(RefreshLines, System.Windows.Threading.DispatcherPriority.Loaded);
            };
            Closed += (_, __) => viewModel.RequestLinesRefresh -= RefreshLines;
        }

        private void RefreshLines()
        {
            var vm = (MainWindowViewModel)DataContext;

            foreach (var line in _lines)
                LinesCanvas.Children.Remove(line);
            _lines.Clear();

            foreach (var (fromNodeId, fromComponentId, toNodeId) in vm.Connections)
            {
                NodeViewModel sourceNode = vm.Nodes.FirstOrDefault(n => n.Model.ID == fromNodeId);
                NodeViewModel targetNode = vm.Nodes.FirstOrDefault(n => n.Model.ID == toNodeId);
                if (sourceNode == null || targetNode == null) continue;

                // Find the visual containers for both nodes
                var sourceContainer = NodesControl.ItemContainerGenerator.ContainerFromItem(sourceNode) as FrameworkElement;
                var targetContainer = NodesControl.ItemContainerGenerator.ContainerFromItem(targetNode) as FrameworkElement;

                if (sourceContainer == null || targetContainer == null) continue;

                // Calculate X1, Y1 based on the button if found, otherwise fallback to standard offset
                double x1 = 0, y1 = 0, x2 = 0, y2 = 0;

                // Attempt to find the outgoing button on the source component
                var componentViewModel = sourceNode.ConnectionComponents.FirstOrDefault(c => c.ID == fromComponentId);
                Button outgoingButton = null;
                if (componentViewModel != null)
                {
                    outgoingButton = FindVisualChild<Button>(sourceContainer, btn =>
                        btn.DataContext == componentViewModel && (btn.Content as string) == "Outgoing");
                }

                if (outgoingButton != null)
                {
                    Point btnPos = outgoingButton.TransformToVisual(LinesCanvas).Transform(new Point(outgoingButton.ActualWidth, outgoingButton.ActualHeight / 2));
                    x1 = btnPos.X;
                    y1 = btnPos.Y;
                }
                else
                {
                    x1 = sourceNode.X;
                    y1 = sourceNode.Y; // fallback
                }

                // Calculate X2, Y2 based on the target node's visual bounds
                Point targetPos = targetContainer.TransformToVisual(LinesCanvas).Transform(new Point(0, targetContainer.ActualHeight / 2));
                x2 = targetPos.X;
                y2 = targetPos.Y;

                var backLine = new Line
                {
                    Stroke = Brushes.Blue,
                    StrokeThickness = 5,
                    X1 = x1,
                    Y1 = y1,
                    X2 = x2,
                    Y2 = y2,
                };

                var frontLine = new Line
                {
                    Stroke = Brushes.LightBlue,
                    StrokeThickness = 3,
                    X1 = x1,
                    Y1 = y1,
                    X2 = x2,
                    Y2 = y2,
                };
                

                _lines.Add(backLine);
                LinesCanvas.Children.Add(backLine);

                _lines.Add(frontLine);
                LinesCanvas.Children.Add(frontLine);
            }
        }

        // Helper function to recursively find a specific child element in the visual tree
        private static T FindVisualChild<T>(DependencyObject parent, System.Func<T, bool> condition = null) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child != null && child is T typedChild)
                {
                    if (condition == null || condition(typedChild))
                        return typedChild;
                }

                T childOfChild = FindVisualChild<T>(child, condition);
                if (childOfChild != null)
                    return childOfChild;
            }
            return null;
        }
    }
}