using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Honours_Stage_Project.Models
{
    public class NodeModel : INotifyPropertyChanged
    {
        private int _id;
        private string _textContent = string.Empty;
        private double _x = 50;
        private double _y = 50;


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
            Point position = new Point(X, Y);
            return new { ID, TextContent, Connections = exportedConnections, position };
        }

        public void Import(dynamic data)
        {
            TextContent = data.TextContent;
            X = (double)data.position.X;
            Y = (double)data.position.Y;
            ConnectionComponents.Clear();
            foreach (var connData in data.Connections)
            {
                var component = AddConnectionComponent();
                component.Import(connData);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
