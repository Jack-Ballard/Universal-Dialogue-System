using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Honours_Stage_Project.Node
{
    public class TextBoxViewModel
    {
        protected TextBoxModel _textBoxModel;
        protected TextBoxView _textBoxView;
        protected MainWindow _window;
        public TextBoxViewModel(MainWindow window)
        {
            _textBoxModel = new TextBoxModel();
            _textBoxView = new TextBoxView(this);
            _window = window;

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
        public void AddOutgoingConnections(object sender)
        {
            // sender is the Button
            var button = sender as Button;
            if (button == null) return;

            // DataContext is the ComponentConnection for this item
            var connection = button.DataContext as ComponentConnection;
            if (connection == null) return;

            // Now you can access the ID (assuming you have a public property)
            int id = connection.ID; // Make sure Id is public in ComponentConnection

            NodeConnections.AddOutgoingConnection(this, id);
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
