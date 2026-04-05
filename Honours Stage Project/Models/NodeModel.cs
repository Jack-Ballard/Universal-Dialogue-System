using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

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
            return new { ID, TextContent, Connections = exportedConnections };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
