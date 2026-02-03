using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        // Enum for resizing direction (edges & corners)
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
        public ObservableCollection<ComponentConnection> TextBoxConnectionComponent { get; set; } = new ObservableCollection<ComponentConnection>();

        private Canvas _parentCanvas;
        private TextBoxViewModel _textBoxViewModel;

        public TextBoxView(TextBoxViewModel TextBoxViewModel)
        {
            InitializeComponent();
            _textBoxViewModel = TextBoxViewModel;
            ConnectionComponent.ItemsSource = TextBoxConnectionComponent;
        }

        // Mouse down event: Start move or resize
        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _parentCanvas = this.Parent as Canvas;
            if (_parentCanvas == null) return;

            var mousePos = e.GetPosition(_parentCanvas);
            _mouseStartPoint = mousePos;
            _resizeDirection = GetResizeDirection(e.GetPosition(this));

            if (_resizeDirection != ResizeDirection.None)
            {
                _initialSize = new Size(Width, Height);
                _initialPosition = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
                _isResizing = true;
                CaptureMouse();
            }
            else
            {
                _initialPosition = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
                _isMoving = true;
                CaptureMouse();
            }
        }

        // Mouse move event: Handle resizing or moving
        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (_parentCanvas == null) return;

            var mousePos = e.GetPosition(_parentCanvas);
            if (_isResizing)
                ResizeControl(mousePos);
            else if (_isMoving)
                MoveControl(mousePos);
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

            double newWidth = _initialSize.Width;
            double newHeight = _initialSize.Height;
            double newLeft = _initialPosition.X;
            double newTop = _initialPosition.Y;

            switch (_resizeDirection)
            {
                case ResizeDirection.Top:
                    newHeight = _initialSize.Height - deltaY;
                    newTop = _initialPosition.Y + deltaY;
                    break;
                case ResizeDirection.Bottom:
                    newHeight = _initialSize.Height + deltaY;
                    break;
                case ResizeDirection.Left:
                    newWidth = _initialSize.Width - deltaX;
                    newLeft = _initialPosition.X + deltaX;
                    break;
                case ResizeDirection.Right:
                    newWidth = _initialSize.Width + deltaX;
                    break;
                case ResizeDirection.TopLeft:
                    newWidth = _initialSize.Width - deltaX;
                    newHeight = _initialSize.Height - deltaY;
                    newLeft = _initialPosition.X + deltaX;
                    newTop = _initialPosition.Y + deltaY;
                    break;
                case ResizeDirection.TopRight:
                    newWidth = _initialSize.Width + deltaX;
                    newHeight = _initialSize.Height - deltaY;
                    newTop = _initialPosition.Y + deltaY;
                    break;
                case ResizeDirection.BottomLeft:
                    newWidth = _initialSize.Width - deltaX;
                    newHeight = _initialSize.Height + deltaY;
                    newLeft = _initialPosition.X + deltaX;
                    break;
                case ResizeDirection.BottomRight:
                    newWidth = _initialSize.Width + deltaX;
                    newHeight = _initialSize.Height + deltaY;
                    break;
            }

            if (newWidth > 20)
            {
                Width = Math.Round(newWidth, 2);
                Canvas.SetLeft(this, newLeft);
            }
            if (newHeight > 20)
            {
                Height = Math.Round(newHeight, 2);
                Canvas.SetTop(this, newTop);
            }
        }

        private void MoveControl(Point mousePos)
        {
            double deltaX = mousePos.X - _mouseStartPoint.X;
            double deltaY = mousePos.Y - _mouseStartPoint.Y;
            Canvas.SetLeft(this, _initialPosition.X + deltaX);
            Canvas.SetTop(this, _initialPosition.Y + deltaY);
        }

        private void AddConnection_Click(object sender, RoutedEventArgs e)
        {
            _textBoxViewModel.AddConectionComponent();
        }

        public void AddConnectionComponent(ComponentConnection componentConnection)
        {
            TextBoxConnectionComponent.Add(componentConnection);
        }
        private void OutgoingButton_Click(object sender, RoutedEventArgs e)
        {
            _textBoxViewModel.AddOutgoingConnections(sender);
        }

        private void IncommingButton_Click(object sender, RoutedEventArgs e)
        {
            _textBoxViewModel.AddIncommingConnections();
        }
    }
}
