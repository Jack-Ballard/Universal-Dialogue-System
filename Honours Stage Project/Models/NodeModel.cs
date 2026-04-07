using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

namespace Honours_Stage_Project.Models
{
    public class NodeModel : INotifyPropertyChanged
    {
        private int _id;
        private string _textContent = string.Empty;
        private double _x = 50;
        private double _y = 50;
        private Size _size = new Size(280, 200);


        public int ID
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(ID)); }
        }

        public string TextContent
        {
            get => _textContent;
            set { _textContent = value; OnPropertyChanged(nameof(TextContent)); }
        }

        public double X
        {
            get => _x;
            set { _x = value; OnPropertyChanged(nameof(X)); }
        }

        public double Y
        {
            get => _y;
            set { _y = value; OnPropertyChanged(nameof(Y)); }
        }

        public Size Size
        {
            get => _size;
            set { _size = value; OnPropertyChanged(nameof(Size)); }
        }

        public ObservableCollection<AttributeModel> Attributes = new ObservableCollection<AttributeModel>();

        public ObservableCollection<ConnectionModel> ConnectionComponents { get; } = new ObservableCollection<ConnectionModel>();

        public NodeModel(int id)
        {
            // Assign the backing field directly during construction so that
            // observers (which don't exist yet) are not notified unnecessarily.
            _id = id;
        }

        public ConnectionModel AddConnectionComponent()
        {
            var component = new ConnectionModel(ConnectionComponents.Count+1);
            ConnectionComponents.Add(component);
            return component;
        }

        public ConnectionModel GetComponentConnection(int id)
            => ConnectionComponents.FirstOrDefault(c => c.ID == id);

        public object Export()
        {
            var exportedConnections = new List<object>();
            foreach (var connection in ConnectionComponents)
                exportedConnections.Add(connection.Export());
            var exportedAttributes = new List<object>();
            foreach (var attribute in Attributes)
                exportedAttributes.Add(attribute.Export());
            Point position = new Point(X, Y);

            return new { ID, TextContent, Attributes = exportedAttributes, Connections = exportedConnections, position, Size };
        }

        public void Import(dynamic data)
        {
            TextContent = data.TextContent;
            ParseGeometry(data.position, out double x, out double y);
            X = x;
            Y = y;
            ParseGeometry(data.Size, out double width, out double height);
            Size = new Size(width, height);
            ConnectionComponents.Clear();
            foreach (var connData in data.Connections)
            {
                var component = AddConnectionComponent();
                component.Import(connData);
            }
            foreach (var attrData in data.Attributes)
            {
                var attribute = new AttributeModel();
                attribute.Import(attrData);
                Attributes.Add(attribute);
            }
        }

        private static void ParseGeometry(JToken positionToken, out double x, out double y)
        {
            x = 50;
            y = 50;

            if (positionToken == null)
                return;

            if (positionToken.Type == JTokenType.String)
            {
                var parts = positionToken.ToString().Split(',');
                if (parts.Length == 2)
                {
                    if (TryParseDouble(parts[0], out var parsedX))
                        x = parsedX;
                    if (TryParseDouble(parts[1], out var parsedY))
                        y = parsedY;
                }

                return;
            }

            if (positionToken.Type == JTokenType.Object)
            {
                x = (double?)positionToken["X"] ?? x;
                y = (double?)positionToken["Y"] ?? y;
            }
        }

        private static bool TryParseDouble(string input, out double value)
        {
            return double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                   || double.TryParse(input, NumberStyles.Float, CultureInfo.CurrentCulture, out value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
