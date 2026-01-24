using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Honours_Stage_Project.Node
{
    /// <summary>  
    /// Interaction logic for TextBoxView.xaml  
    /// </summary>  
    public partial class TextBoxView : UserControl
    {
        private Point _mouseStartPoint;
        private bool _isResizing;
        private bool _isMoving;
        private ResizeDirection _resizeDirection;
        private Size _initialSize;
        private Point _initialPosition;

        private TextBoxViewModel _textBoxViewModel;

        // Enum for resizing direction (edges & corners)
        private enum ResizeDirection
        {
            None,
            Top, Bottom, Left, Right,
            TopLeft, TopRight, BottomLeft, BottomRight
        }

        public TextBoxView(TextBoxViewModel TextBoxViewModel)
        {
            InitializeComponent();
            _textBoxViewModel = TextBoxViewModel;
        }

        // Mouse down event: Start move or resize
        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(this);
            _mouseStartPoint = mousePos;
            _resizeDirection = GetResizeDirection(mousePos);

            // Capture mouse for resizing or moving
            if (_resizeDirection != ResizeDirection.None)
            {
                // It's a resize action
                _initialSize = new Size(Width, Height);
                _initialPosition = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
                _isResizing = true;
                CaptureMouse();
            }
            else
            {
                // It's a move action
                _initialPosition = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
                _isMoving = true;
                CaptureMouse();
            }
        }

        // Mouse move event: Handle resizing or moving
        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isResizing)
                ResizeControl(e.GetPosition(this));
            else if (_isMoving)
                MoveControl(e.GetPosition(this));
        }

        // Mouse up event: End move or resize
        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isResizing = _isMoving = false;
            ReleaseMouseCapture();
        }

        // Determine resizing direction based on mouse position
        private ResizeDirection GetResizeDirection(Point mousePos)
        {
            const double tolerance = 10; // Distance from edges to trigger resizing
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

        // Handle resizing based on the resize direction
        private void ResizeControl(Point mousePos)
        {
            double deltaX = mousePos.X - _mouseStartPoint.X;
            double deltaY = mousePos.Y - _mouseStartPoint.Y;

            // Resize based on direction
            switch (_resizeDirection)
            {
                case ResizeDirection.Top:
                    Height = _initialSize.Height - deltaY;
                    Canvas.SetTop(this, Canvas.GetTop(this) + deltaY); // Move control upwards
                    break;
                case ResizeDirection.Bottom:
                    Height = _initialSize.Height + deltaY;
                    break;
                case ResizeDirection.Left:
                    Width = _initialSize.Width - deltaX;
                    Canvas.SetLeft(this, _initialPosition.X + deltaX); // Move control leftwards
                    break;
                case ResizeDirection.Right:
                    Width = _initialSize.Width + deltaX;
                    break;
                case ResizeDirection.TopLeft:
                    Width = _initialSize.Width - deltaX;
                    Height = _initialSize.Height - deltaY;
                    Canvas.SetLeft(this, _initialPosition.X + deltaX); // Move leftwards
                    Canvas.SetTop(this, _initialPosition.Y + deltaY); // Move upwards
                    break;
                case ResizeDirection.TopRight:
                    Width = _initialSize.Width + deltaX;
                    Height = _initialSize.Height - deltaY;
                    Canvas.SetTop(this, _initialPosition.Y + deltaY); // Move upwards
                    break;
                case ResizeDirection.BottomLeft:
                    Width = _initialSize.Width - deltaX;
                    Height = _initialSize.Height + deltaY;
                    Canvas.SetLeft(this, _initialPosition.X + deltaX); // Move leftwards
                    break;
                case ResizeDirection.BottomRight:
                    Width = _initialSize.Width + deltaX;
                    Height = _initialSize.Height + deltaY;
                    break;
            }
            Width = Math.Round(Width, 2);
            Height = Math.Round(Height, 2);
        }

        // Handle moving the control
        private void MoveControl(Point mousePos)
        {
            double deltaX = mousePos.X - _mouseStartPoint.X;
            double deltaY = mousePos.Y - _mouseStartPoint.Y;

            // Check current position before updating
            double currentLeft = Canvas.GetLeft(this);
            double currentTop = Canvas.GetTop(this);

            // Apply the delta, making sure the position remains in expected bounds
            double newLeft = currentLeft + deltaX;
            double newTop = currentTop + deltaY;

            // Update position on the canvas
            Canvas.SetLeft(this, newLeft);
            Canvas.SetTop(this, newTop);
        }

        // Handle moving the control
        //private void MoveControl(Point mousePos)
        //{
        //    double deltaX = mousePos.X - _mouseStartPoint.X;
        //    double deltaY = mousePos.Y - _mouseStartPoint.Y;
        //    Canvas.SetLeft(this, _initialPosition.X + deltaX);
        //    Canvas.SetTop(this, _initialPosition.Y + deltaY);
        //}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //var mainWindow = FindParent<MainWindow>(this);
            //mainWindow.ConnectNodes(this);
            _textBoxViewModel.UpdateConnections();
        }
        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            // Go up the tree and find the first parent of type T
            while (child != null)
            {
                if (child is T parent)
                {
                    return parent;
                }
                child = VisualTreeHelper.GetParent(child);
            }
            return null;
        }
    }
}
