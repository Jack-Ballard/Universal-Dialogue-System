using Honours_Stage_Project.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace Honours_Stage_Project.Node
{
    public class TextBoxViewModel
    {
        protected TextBoxModel _textBoxModel;
        protected TextBoxView _textBoxView;
        protected MainWindow _window;
        public ICommand AddOutwardConnectionCommand { get; }


        public TextBoxViewModel(MainWindow window, int ID)
        {
            _textBoxModel = new TextBoxModel(ID);
            _textBoxView = new TextBoxView(this);
            _window = window;

            AddOutwardConnectionCommand = new RelayCommand(param => AddOutgoingConnections(param as ComponentConnection));

            InitaliseTextBox();
        }

        protected void InitaliseTextBox()
        {
            Canvas.SetLeft(_textBoxView, 50);
            Canvas.SetTop(_textBoxView, 50);

            // Add the new control to the Canvas
            _window.MyCanvas.Children.Add(_textBoxView);
        }

        public void Update()
        {
            _textBoxModel.TextContent = _textBoxView.InputTextBox.Text;
        }

        public TextBoxModel GetTextBoxModel()
        {
            return _textBoxModel;
        }
        public TextBoxView GetTextBoxView()
        {
            return _textBoxView;
        }
        public void AddOutgoingConnections(ComponentConnection connection)
        {
            NodeConnections.AddOutgoingConnection(_textBoxModel.ID, connection.ID);
        }
        public void AddIncomingConnections()
        {
            NodeConnections.AddIncomingConnection(_textBoxModel.ID);
        }
        public void AddConectionComponent()
        {
            _textBoxModel.AddConnectionComponent();
        }
        public Point GetTextboxPosition()
        {

            double x = Canvas.GetLeft(_textBoxView);
            double y = Canvas.GetTop(_textBoxView); 
            return new Point(x, y);
        }

        public Point GetConnectionComponentButtonPosition(int index)
        {
            var container = _textBoxView.ConnectionComponent.ItemContainerGenerator.ContainerFromIndex(index) as ContentPresenter;
            if (container != null)
            {
                var button = FindVisualChildren.FindVisualChild<Button>(container);
                if (button != null)
                {
                    Point position = button.TransformToAncestor(_textBoxView)
                        .Transform(new Point(0, 50));
                    return position;
                }
            }
            return new Point(0, 0);
        }
    }
}
