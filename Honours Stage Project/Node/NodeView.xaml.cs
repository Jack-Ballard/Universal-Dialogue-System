using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
        private Size _initialSize;
        private Point _initialPosition;

        private NodeViewModel ViewModel => DataContext as NodeViewModel;

        public NodeView()
        {
            InitializeComponent();
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
            var parentCanvas = FindParentCanvas();
            if (parentCanvas == null) return;

            var mousePos = e.GetPosition(parentCanvas);
            _mouseStartPoint = mousePos;
            _resizeDirection = GetResizeDirection(e.GetPosition(this));

            if (_resizeDirection != ResizeDirection.None)
            {
                _initialSize = new Size(Width, Height);
                _initialPosition = new Point(ViewModel?.X ?? 0, ViewModel?.Y ?? 0);
                _isResizing = true;
                CaptureMouse();
            }
            else
            {
                _initialPosition = new Point(ViewModel?.X ?? 0, ViewModel?.Y ?? 0);
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
            ReleaseMouseCapture();
        }

        private ResizeDirection GetResizeDirection(Point mousePos)
        {
            const double tolerance = 10;
            if (mousePos.X <= tolerance && mousePos.Y <= tolerance)               return ResizeDirection.TopLeft;
            if (mousePos.X >= ActualWidth - tolerance && mousePos.Y <= tolerance) return ResizeDirection.TopRight;
            if (mousePos.X <= tolerance && mousePos.Y >= ActualHeight - tolerance) return ResizeDirection.BottomLeft;
            if (mousePos.X >= ActualWidth - tolerance && mousePos.Y >= ActualHeight - tolerance) return ResizeDirection.BottomRight;
            if (mousePos.X <= tolerance)               return ResizeDirection.Left;
            if (mousePos.X >= ActualWidth - tolerance) return ResizeDirection.Right;
            if (mousePos.Y <= tolerance)               return ResizeDirection.Top;
            if (mousePos.Y >= ActualHeight - tolerance) return ResizeDirection.Bottom;
            return ResizeDirection.None;
        }

        private void ResizeControl(Point mousePos)
        {
            double deltaX = mousePos.X - _mouseStartPoint.X;
            double deltaY = mousePos.Y - _mouseStartPoint.Y;

            double newWidth  = _initialSize.Width;
            double newHeight = _initialSize.Height;
            double newLeft   = _initialPosition.X;
            double newTop    = _initialPosition.Y;

            switch (_resizeDirection)
            {
                case ResizeDirection.Top:
                    newHeight = _initialSize.Height - deltaY;
                    newTop    = _initialPosition.Y  + deltaY;
                    break;
                case ResizeDirection.Bottom:
                    newHeight = _initialSize.Height + deltaY;
                    break;
                case ResizeDirection.Left:
                    newWidth = _initialSize.Width - deltaX;
                    newLeft  = _initialPosition.X + deltaX;
                    break;
                case ResizeDirection.Right:
                    newWidth = _initialSize.Width + deltaX;
                    break;
                case ResizeDirection.TopLeft:
                    newWidth  = _initialSize.Width  - deltaX;
                    newHeight = _initialSize.Height - deltaY;
                    newLeft   = _initialPosition.X  + deltaX;
                    newTop    = _initialPosition.Y  + deltaY;
                    break;
                case ResizeDirection.TopRight:
                    newWidth  = _initialSize.Width  + deltaX;
                    newHeight = _initialSize.Height - deltaY;
                    newTop    = _initialPosition.Y  + deltaY;
                    break;
                case ResizeDirection.BottomLeft:
                    newWidth  = _initialSize.Width  - deltaX;
                    newHeight = _initialSize.Height + deltaY;
                    newLeft   = _initialPosition.X  + deltaX;
                    break;
                case ResizeDirection.BottomRight:
                    newWidth  = _initialSize.Width  + deltaX;
                    newHeight = _initialSize.Height + deltaY;
                    break;
            }

            if (newWidth > 20)
            {
                Width = System.Math.Round(newWidth, 2);
                if (ViewModel != null) ViewModel.X = newLeft;
            }
            if (newHeight > 20)
            {
                Height = System.Math.Round(newHeight, 2);
                if (ViewModel != null) ViewModel.Y = newTop;
            }
        }

        private void MoveControl(Point mousePos)
        {
            double deltaX = mousePos.X - _mouseStartPoint.X;
            double deltaY = mousePos.Y - _mouseStartPoint.Y;
            if (ViewModel != null)
            {
                ViewModel.X = _initialPosition.X + deltaX;
                ViewModel.Y = _initialPosition.Y + deltaY;
            }
        }
    }
}
