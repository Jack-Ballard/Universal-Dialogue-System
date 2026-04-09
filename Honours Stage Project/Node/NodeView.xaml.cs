using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Honours_Stage_Project.ViewModels;

namespace Honours_Stage_Project.Node
{
    /// <summary>
    /// View for a single dialogue node. All interaction logic lives here; domain
    /// state is read from / written to the NodeViewModel via DataContext.
    /// </summary>
    public partial class NodeView : UserControl
    {
        private enum ResizeDirection
        {
            None,
            Top, Bottom, Left, Right,
            TopLeft, TopRight, BottomLeft, BottomRight
        }

        private Point _mouseStartPoint;
        private bool _isResizing;
        private bool _isMoving;
        private ResizeDirection _resizeDirection;
        private Point _position;
        private Size _resizeStartSize;

        private double _lastConnectionComponentsHeight;

        private NodeViewModel ViewModel => DataContext as NodeViewModel;

        public NodeView()
        {
            InitializeComponent();

            Loaded += NodeView_Loaded;
            Unloaded += NodeView_Unloaded;
            DataContextChanged += NodeView_DataContextChanged;
        }

        private void NodeView_Loaded(object sender, RoutedEventArgs e)
        {
            SubscribeToNode(ViewModel);
            ApplySizeFromViewModel();
            CaptureConnectionComponentsHeightBaseline();
        }

        private void NodeView_Unloaded(object sender, RoutedEventArgs e)
        {
            UnsubscribeFromNode(ViewModel);
        }

        private void NodeView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is NodeViewModel oldVm)
                UnsubscribeFromNode(oldVm);

            if (e.NewValue is NodeViewModel newVm)
            {
                SubscribeToNode(newVm);

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    ApplySizeFromViewModel();
                    CaptureConnectionComponentsHeightBaseline();
                }), DispatcherPriority.Loaded);
            }
        }

        private void ApplySizeFromViewModel()
        {
            if (ViewModel == null)
                return;

            if (ViewModel.Size.Width > 0)
                Width = Math.Round(ViewModel.Size.Width, 2);

            if (ViewModel.Size.Height > 0)
                Height = Math.Round(ViewModel.Size.Height, 2);

            if (InputTextBox != null)
            {
                InputTextBox.Width = Math.Max(0,Width - 40);
                InputTextBox.Height = Math.Max(0,Height - 110 - GetTotalDynamicContentHeight());
            }
        }

        private void SubscribeToNode(NodeViewModel nodeViewModel)
        {
            if (nodeViewModel == null) return;

            nodeViewModel.ConnectionComponents.CollectionChanged -= ConnectionComponents_CollectionChanged;
            nodeViewModel.ConnectionComponents.CollectionChanged += ConnectionComponents_CollectionChanged;

            nodeViewModel.Attributes.CollectionChanged -= NodeAttributes_CollectionChanged;
            nodeViewModel.Attributes.CollectionChanged += NodeAttributes_CollectionChanged;

            foreach (var component in nodeViewModel.ConnectionComponents)
                SubscribeToConnection(component);
        }

        private void UnsubscribeFromNode(NodeViewModel nodeViewModel)
        {
            if (nodeViewModel == null) return;

            nodeViewModel.ConnectionComponents.CollectionChanged -= ConnectionComponents_CollectionChanged;
            nodeViewModel.Attributes.CollectionChanged -= NodeAttributes_CollectionChanged;

            foreach (var component in nodeViewModel.ConnectionComponents)
                UnsubscribeFromConnection(component);
        }

        private void SubscribeToConnection(ConnectionViewModel connection)
        {
            if (connection == null) return;

            connection.Attributes.CollectionChanged -= ConnectionChildren_CollectionChanged;
            connection.Attributes.CollectionChanged += ConnectionChildren_CollectionChanged;

            connection.Conditions.CollectionChanged -= ConnectionChildren_CollectionChanged;
            connection.Conditions.CollectionChanged += ConnectionChildren_CollectionChanged;

            connection.OutgoingConnections.CollectionChanged -= ConnectionChildren_CollectionChanged;
            connection.OutgoingConnections.CollectionChanged += ConnectionChildren_CollectionChanged;

            foreach (var outgoing in connection.OutgoingConnections)
                SubscribeToOutgoingConnection(outgoing);
        }

        private void UnsubscribeFromConnection(ConnectionViewModel connection)
        {
            if (connection == null) return;

            connection.Attributes.CollectionChanged -= ConnectionChildren_CollectionChanged;
            connection.Conditions.CollectionChanged -= ConnectionChildren_CollectionChanged;

            connection.OutgoingConnections.CollectionChanged -= ConnectionChildren_CollectionChanged;

            foreach (var outgoing in connection.OutgoingConnections)
                UnsubscribeFromOutgoingConnection(outgoing);
        }

        private void SubscribeToOutgoingConnection(OutgoingConnectionViewModel outgoingConnection)
        {
            if (outgoingConnection == null) return;

            outgoingConnection.Conditions.CollectionChanged -= ConnectionChildren_CollectionChanged;
            outgoingConnection.Conditions.CollectionChanged += ConnectionChildren_CollectionChanged;
        }

        private void UnsubscribeFromOutgoingConnection(OutgoingConnectionViewModel outgoingConnection)
        {
            if (outgoingConnection == null) return;

            outgoingConnection.Conditions.CollectionChanged -= ConnectionChildren_CollectionChanged;
        }

        private void ConnectionComponents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (ConnectionViewModel component in e.OldItems)
                    UnsubscribeFromConnection(component);
            }

            if (e.NewItems != null)
            {
                foreach (ConnectionViewModel component in e.NewItems)
                    SubscribeToConnection(component);
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
                QueueGrowTextBoxByConnectionDelta();
            else
                QueueRefreshConnectionHeightBaseline();
        }

        private void ConnectionChildren_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                QueueGrowTextBoxByConnectionDelta();
            else
                QueueRefreshConnectionHeightBaseline();
        }

        // Add this method to handle the CollectionChanged event for Node Attributes
        private void NodeAttributes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // If attributes are added, grow the textbox by the delta
            if (e.Action == NotifyCollectionChangedAction.Add)
                QueueGrowTextBoxByConnectionDelta();
            else
                QueueRefreshConnectionHeightBaseline();
        }

        private void QueueGrowTextBoxByConnectionDelta()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var currentHeight = GetTotalDynamicContentHeight();
                var delta = currentHeight - _lastConnectionComponentsHeight;

                if (delta > 0)
                    Height = System.Math.Round(Height + delta, 2);

                _lastConnectionComponentsHeight = currentHeight;
            }), DispatcherPriority.Loaded);
        }

        private void QueueRefreshConnectionHeightBaseline()
        {
            Dispatcher.BeginInvoke(new Action(CaptureConnectionComponentsHeightBaseline), DispatcherPriority.Loaded);
        }

        private void CaptureConnectionComponentsHeightBaseline()
        {
            _lastConnectionComponentsHeight = GetTotalDynamicContentHeight();
        }

        private double GetTotalDynamicContentHeight()
        {
            return GetTotalConnectionComponentsHeight() + GetTotalAttributeHeight();
        }

        private double GetTotalAttributeHeight()
        {
            if (NodeAttributes == null)
                return 0;

            NodeAttributes.UpdateLayout();

            double total = 0;

            foreach (var item in NodeAttributes.Items)
            {
                var container = NodeAttributes.ItemContainerGenerator.ContainerFromItem(item) as ContentPresenter;
                if (container != null)
                    total += container.ActualHeight;
            }

            return total;
        }

        // ── Drag / resize ────────────────────────────────────────────────────

        private Canvas FindParentCanvas()
        {
            DependencyObject parent = VisualTreeHelper.GetParent(this);
            while (parent != null)
            {
                if (parent is Canvas canvas)
                    return canvas;
                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IInputElement ParentCanvas = FindParentCanvas();
            if (ParentCanvas == null) return;

            var mousePos = e.GetPosition(ParentCanvas);
            _mouseStartPoint = mousePos;
            _resizeDirection = GetResizeDirection(e.GetPosition(this));

            if (_resizeDirection != ResizeDirection.None)
            {
                _position = new Point(ViewModel?.X ?? 0, ViewModel?.Y ?? 0);
                _resizeStartSize = new Size(Width, Height);
                _isResizing = true;
                CaptureMouse();
            }
            else
            {
                _position = new Point(ViewModel?.X ?? 0, ViewModel?.Y ?? 0);
                _isMoving = true;
                CaptureMouse();
            }

        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            var parentCanvas = FindParentCanvas();
            if (parentCanvas == null) return;

            var mousePos = e.GetPosition(parentCanvas);
            if (_isResizing)
                ResizeControl(mousePos);
            else if (_isMoving)
                MoveControl(mousePos);
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isResizing = _isMoving = false;
            ViewModel.Size = new Size(Width, Height);
            //ViewModel.TextContent = ("Width: " + ViewModel.Size.Width + "\nHeight: " + ViewModel.Size.Height); // Debug: show size in text content

            ReleaseMouseCapture();
        }

        private ResizeDirection GetResizeDirection(Point mousePos)
        {
            const double tolerance = 10;
            if (mousePos.X <= tolerance && mousePos.Y <= tolerance) return ResizeDirection.TopLeft;
            if (mousePos.X >= ActualWidth - tolerance && mousePos.Y <= tolerance) return ResizeDirection.TopRight;
            if (mousePos.X <= tolerance && mousePos.Y >= ActualHeight - tolerance) return ResizeDirection.BottomLeft;
            if (mousePos.X >= ActualWidth - tolerance && mousePos.Y >= ActualHeight - tolerance) return ResizeDirection.BottomRight;
            if (mousePos.X <= tolerance) return ResizeDirection.Left;
            if (mousePos.X >= ActualWidth - tolerance) return ResizeDirection.Right;
            if (mousePos.Y <= tolerance) return ResizeDirection.Top;
            if (mousePos.Y >= ActualHeight - tolerance) return ResizeDirection.Bottom;
            return ResizeDirection.None;
        }

        private void ResizeControl(Point mousePos)
        {
            double deltaX = mousePos.X - _mouseStartPoint.X;
            double deltaY = mousePos.Y - _mouseStartPoint.Y;

            double newWidth = _resizeStartSize.Width;
            double newHeight = _resizeStartSize.Height;
            double newLeft = _position.X;
            double newTop = _position.Y;

            switch (_resizeDirection)
            {
                case ResizeDirection.Top:
                    newHeight = _resizeStartSize.Height - deltaY;
                    newTop = _position.Y + deltaY;
                    break;
                case ResizeDirection.Bottom:
                    newHeight = _resizeStartSize.Height + deltaY;
                    break;
                case ResizeDirection.Left:
                    newWidth = _resizeStartSize.Width - deltaX;
                    newLeft = _position.X + deltaX;
                    break;
                case ResizeDirection.Right:
                    newWidth = _resizeStartSize.Width + deltaX;
                    break;
                case ResizeDirection.TopLeft:
                    newWidth = _resizeStartSize.Width - deltaX;
                    newHeight = _resizeStartSize.Height - deltaY;
                    newLeft = _position.X + deltaX;
                    newTop = _position.Y + deltaY;
                    break;
                case ResizeDirection.TopRight:
                    newWidth = _resizeStartSize.Width + deltaX;
                    newHeight = _resizeStartSize.Height - deltaY;
                    newTop = _position.Y + deltaY;
                    break;
                case ResizeDirection.BottomLeft:
                    newWidth = _resizeStartSize.Width - deltaX;
                    newHeight = _resizeStartSize.Height + deltaY;
                    newLeft = _position.X + deltaX;
                    break;
                case ResizeDirection.BottomRight:
                    newWidth = _resizeStartSize.Width + deltaX;
                    newHeight = _resizeStartSize.Height + deltaY;
                    break;
            }

            if (newWidth > 240)
            {
                Width = Math.Round(newWidth, 2);
                if (ViewModel != null)
                { 
                    ViewModel.X = newLeft; 
                    TopBar.Width = Width - 90;
                }
                if (InputTextBox != null) 
                { 
                    InputTextBox.Width = Width - 40; 
                }
            }
            if (newHeight > 180 + GetTotalDynamicContentHeight())
            {
                Height = Math.Round(newHeight, 2);
                if (ViewModel != null) ViewModel.Y = newTop;
                if (InputTextBox != null) InputTextBox.Height = Height - 110 - GetTotalDynamicContentHeight();
            }

            if (ViewModel != null)
                ViewModel.Size = new Size(Width, Height);
        }

        private void MoveControl(Point mousePos)
        {
            double deltaX = mousePos.X - _mouseStartPoint.X;
            double deltaY = mousePos.Y - _mouseStartPoint.Y;
            if (ViewModel != null)
            {
                ViewModel.X = _position.X + deltaX;
                ViewModel.Y = _position.Y + deltaY;
            }
        }

        private IReadOnlyList<Size> GetConnectionComponentSizes()
        {
            ConnectionComponent.UpdateLayout();

            var sizes = new List<Size>();

            foreach (var item in ConnectionComponent.Items)
            {
                var container = ConnectionComponent.ItemContainerGenerator.ContainerFromItem(item) as ContentPresenter;
                if (container == null)
                    continue;

                sizes.Add(new Size(container.ActualWidth, container.ActualHeight));
            }

            return sizes;
        }

        private double GetTotalConnectionComponentsHeight()
        {
            var sizes = GetConnectionComponentSizes();
            double total = 0;

            foreach (var size in sizes)
                total += size.Height;

            return total;
        }
    }
}
