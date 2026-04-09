using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Honours_Stage_Project.Node;
using Honours_Stage_Project.ViewModels;

namespace Honours_Stage_Project
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly List<Line> _lines = new List<Line>();

        private bool _isPanning;
        private Point _panStart;
        private Point _panOrigin;
        private const double ZoomStep = 1.1;
        private const double MinZoom = 0.2;
        private const double MaxZoom = 3.5;

        public Point ViewportCenterWorldPoint
        {
            get
            {
                var viewportCenter = new Point(WorldViewport.ActualWidth / 2.0, WorldViewport.ActualHeight / 2.0);
                return new Point(
                    (viewportCenter.X - CameraTranslate.X) / CameraScale.ScaleX,
                    (viewportCenter.Y - CameraTranslate.Y) / CameraScale.ScaleY);
            }
        }

        private void NotifyViewportCenterChanged()
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ViewportCenterWorldPoint)));

        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            WorldViewport.SizeChanged += (_, __) => NotifyViewportCenterChanged();

            viewModel.RequestLinesRefresh += () =>
            {
                Dispatcher.InvokeAsync(RefreshLines, System.Windows.Threading.DispatcherPriority.Loaded);
            };

            // Global camera input (captures events even if child controls mark them handled)
            AddHandler(UIElement.PreviewMouseWheelEvent, new MouseWheelEventHandler(GlobalPreviewMouseWheel), true);
            AddHandler(UIElement.PreviewMouseDownEvent, new MouseButtonEventHandler(GlobalPreviewMouseDown), true);
            AddHandler(UIElement.PreviewMouseMoveEvent, new MouseEventHandler(GlobalPreviewMouseMove), true);
            AddHandler(UIElement.PreviewMouseUpEvent, new MouseButtonEventHandler(GlobalPreviewMouseUp), true);
        }

        private bool IsPointerInsideWorldViewport(InputEventArgs e)
        {
            Point p;

            if (e is MouseEventArgs mouseEventArgs)
                p = mouseEventArgs.GetPosition(WorldViewport);
            else
                return false;

            return p.X >= 0 && p.Y >= 0 && p.X <= WorldViewport.ActualWidth && p.Y <= WorldViewport.ActualHeight;
        }

        private void GlobalPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!IsPointerInsideWorldViewport(e)) return;
            if ((Keyboard.Modifiers & ModifierKeys.Control) == 0) return;

            var oldScale = CameraScale.ScaleX;
            var nextScale = e.Delta > 0 ? oldScale * ZoomStep : oldScale / ZoomStep;
            nextScale = Math.Max(MinZoom, Math.Min(MaxZoom, nextScale));

            if (Math.Abs(nextScale - oldScale) < 0.0001) return;

            var mouse = e.GetPosition(WorldViewport);

            var worldX = (mouse.X - CameraTranslate.X) / oldScale;
            var worldY = (mouse.Y - CameraTranslate.Y) / oldScale;

            CameraScale.ScaleX = nextScale;
            CameraScale.ScaleY = nextScale;

            CameraTranslate.X = mouse.X - (worldX * nextScale);
            CameraTranslate.Y = mouse.Y - (worldY * nextScale);

            NotifyViewportCenterChanged();
            e.Handled = true;
        }

        private void GlobalPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsPointerInsideWorldViewport(e)) return;
            if (e.ChangedButton != MouseButton.Middle) return;

            _isPanning = true;
            _panStart = e.GetPosition(WorldViewport);
            _panOrigin = new Point(CameraTranslate.X, CameraTranslate.Y);
            Mouse.Capture(this);
            e.Handled = true;
        }

        private void GlobalPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isPanning) return;

            var now = e.GetPosition(WorldViewport);
            var delta = now - _panStart;

            CameraTranslate.X = _panOrigin.X + delta.X;
            CameraTranslate.Y = _panOrigin.Y + delta.Y;

            NotifyViewportCenterChanged();
        }

        private void GlobalPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isPanning) return;
            if (e.ChangedButton != MouseButton.Middle) return;

            _isPanning = false;
            Mouse.Capture(null);
            e.Handled = true;
        }

        private void ResetCamera_Click(object sender, RoutedEventArgs e)
        {
            CameraScale.ScaleX = 1;
            CameraScale.ScaleY = 1;
            CameraTranslate.X = 0;
            CameraTranslate.Y = 0;

            NotifyViewportCenterChanged();
        }

        private void RefreshLines()
        {
            var vm = (MainWindowViewModel)DataContext;

            NodesControl.UpdateLayout();

            foreach (var line in _lines)
                LinesCanvas.Children.Remove(line);
            _lines.Clear();

            foreach (var item in vm.Connections)
            {
                var fromNodeId = item.NodeId;
                var fromComponentId = item.ComponentId;
                var fromConnectionId = item.ConnectionId;
                var toNodeId = item.TargetNodeId;

                NodeViewModel sourceNode = vm.Nodes.FirstOrDefault(n => n.Model.ID == fromNodeId);
                NodeViewModel targetNode = vm.Nodes.FirstOrDefault(n => n.Model.ID == toNodeId);
                if (sourceNode == null || targetNode == null) continue;

                var sourceContainer = NodesControl.ItemContainerGenerator.ContainerFromItem(sourceNode) as FrameworkElement;
                var targetContainer = NodesControl.ItemContainerGenerator.ContainerFromItem(targetNode) as FrameworkElement;
                if (sourceContainer == null || targetContainer == null) continue;

                double x1 = 0, y1 = 0, x2 = 0, y2 = 0;

                Button outgoingButton = null;
                var componentViewModel = sourceNode.ConnectionComponents.FirstOrDefault(c => c.ID == fromComponentId);
                if (componentViewModel != null)
                {
                    var outgoingVm = componentViewModel.OutgoingConnections.FirstOrDefault(o => o.ID == fromConnectionId);
                    if (outgoingVm != null)
                    {
                        outgoingButton = FindVisualChild<Button>(sourceContainer, btn =>
                            btn.DataContext == outgoingVm && (btn.Content as string) == "Outgoing");
                    }
                    else if (fromConnectionId == 0)
                    {
                        outgoingButton = FindVisualChild<Button>(sourceContainer, btn =>
                            btn.DataContext == componentViewModel && (btn.Content as string) == "Outgoing");
                    }
                }
                else
                {
                    outgoingButton = FindVisualChild<Button>(sourceContainer, btn =>
                        btn.DataContext == sourceNode && (btn.Content as string) == "Outgoing");
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
                    y1 = sourceNode.Y;
                }

                Button incomingButton = FindVisualChild<Button>(sourceContainer, btn => btn.DataContext == sourceNode && (btn.Content as string) == "Incoming");
                Point targetPos = targetContainer.TransformToVisual(LinesCanvas).Transform(new Point(0, incomingButton.ActualHeight / 2));
                x2 = targetPos.X;
                y2 = targetPos.Y;

                var backLine = new Line
                {
                    Stroke = Brushes.Orange,
                    StrokeThickness = 5,
                    X1 = x1,
                    Y1 = y1,
                    X2 = x2,
                    Y2 = y2,
                };

                var frontLine = new Line
                {
                    Stroke = Brushes.LightYellow,
                    StrokeThickness = 2,
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