using Honours_Stage_Project.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Honours_Stage_Project.Node
{
    public class TextBoxViewModel
    {
        protected TextBoxModel _textBoxModel;
        protected TextBoxView _textBoxView;
        protected MainWindow _window;
        public ICommand AddOutwardConnectionCommand { get; }

        public TextBoxViewModel(MainWindow window)
        {
            _textBoxModel = new TextBoxModel();
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
        public TextBoxView GetTextBoxView()
        {
            return _textBoxView;
        }
        public void AddOutgoingConnections(ComponentConnection connection)
        {
            NodeConnections.AddOutgoingConnection(this, connection);
        }
        public void AddIncommingConnections()
        {
            NodeConnections.AddIncommingConnection(this);
        }
        public void AddConectionComponent()
        {
            _textBoxView.AddConnectionComponent(_textBoxModel.AddConnectionComponent());
        }
    }
}
