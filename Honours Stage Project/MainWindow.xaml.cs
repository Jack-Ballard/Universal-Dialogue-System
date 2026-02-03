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

        public MainWindow()
        {
            InitializeComponent();
            CompositionTarget.Rendering += OnRendering;
        }

        private void OnRendering(object sender, EventArgs e)
        {
            UpdateConnections();
        }

        // This method creates a new TextBoxNode and adds it to the Canvas at runtime
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TextBoxViewModel dynamicControl = new TextBoxViewModel(this);
            //MessageBox.Show("Button was clicked!");
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
                var node1 = connection.Item1.GetTextBoxView();
                int outID = connection.Item2;
                Button button = connection.Item3;
                var node2 = connection.Item4.GetTextBoxView();

                Point test = button.PointToScreen(new Point(-140, -200));

                // Get positions
                //double x1 = Canvas.GetLeft(node1) + node1.ActualWidth;
                //double y1 = Canvas.GetTop(node1) + node1.ActualHeight / 2 + 100 * outID;
                double x1 = test.X;
                double y1 = test.Y;
                double x2 = Canvas.GetLeft(node2);
                double y2 = Canvas.GetTop(node2) + node2.ActualHeight / 2;

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
