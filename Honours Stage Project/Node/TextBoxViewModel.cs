using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public void UpdateConnections()
        {
            _window.ConnectNodes(this);
        }
    }
}
