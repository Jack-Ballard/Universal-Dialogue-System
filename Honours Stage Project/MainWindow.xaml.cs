using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Collections.Generic;
using System;
using Honours_Stage_Project;
using Honours_Stage_Project.Node;

namespace Honours_Stage_Project
{
    public partial class MainWindow : Window
    {
        List<Line> myLine = new List<Line>();
        private List<TextBoxViewModel> _textBoxViewModels = new List<TextBoxViewModel>();

        public MainWindow()
        {
            InitializeComponent();
            CompositionTarget.Rendering += OnRendering;
        }

        private void OnRendering(object sender, EventArgs e)
        {
            UpdateConnections();
            foreach(TextBoxViewModel textBoxViewModel in _textBoxViewModels)
            {
                textBoxViewModel.Update();
            }
        }

        // This method creates a new TextBoxNode and adds it to the Canvas at runtime
        
        private void AddNode_Click(object sender, RoutedEventArgs e)
        {
            TextBoxViewModel dynamicControl = new TextBoxViewModel(this, _textBoxViewModels.Count);
            _textBoxViewModels.Add(dynamicControl);
            //MessageBox.Show("Button was clicked!");
        }
        private void Export_Click(object sender, RoutedEventArgs e)
        {
            List<Object> textBoxData = new List<Object>();
            foreach (TextBoxViewModel textBoxViewModel in _textBoxViewModels)
            {
                textBoxData.Add(textBoxViewModel.GetTextBoxModel().Export());
            }
            List<Object> connectionIDs = NodeConnections.GetConnectionsObject();
            Object dataPackage = new
            {
                TextBoxes = textBoxData,
                Connections = connectionIDs
            };
            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(dataPackage, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText("exported_data.json", jsonData);

        }

        public void UpdateConnections()
        {
            // Remove all existing lines from the canvas
            foreach (var line in myLine)
            {
                MyCanvas.Children.Remove(line);
            }
            myLine.Clear();

            foreach (var connection in NodeConnections.GetConnections())
            {
                var node1 = _textBoxViewModels[connection.Item1];
                //var outID = node1.GetTextBoxModel().GetComponentConnection(connection.Item2);
                var node2 = _textBoxViewModels[connection.Item3];

                // Get positions
                Point position1 = node1.GetTextboxPosition();
                Point position2 = node2.GetTextboxPosition();
                //double x1 = Canvas.GetLeft(node1) + node1.ActualWidth;
                //double y1 = Canvas.GetTop(node1) + node1.ActualHeight / 2 + 100 * outID;
                
                double x1 = position1.X + node1.GetConnectionComponentButtonPosition(connection.Item2).X + node1.GetTextBoxView().ActualWidth / 2;
                double y1 = position1.Y + node1.GetConnectionComponentButtonPosition(connection.Item2).Y; // + node2.ActualHeight / 2;
                double x2 = position2.X;
                double y2 = position2.Y;

                Line newLine = new Line
                {
                    Stroke = Brushes.LightSteelBlue,
                    X1 = x1,
                    Y1 = y1,
                    X2 = x2,
                    Y2 = y2,
                    StrokeThickness = 2,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center
                };

                myLine.Add(newLine);
                MyCanvas.Children.Add(newLine);
            }
        }
    }
}
