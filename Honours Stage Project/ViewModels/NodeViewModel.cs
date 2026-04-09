using Honours_Stage_Project.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Honours_Stage_Project.Helpers;
using Honours_Stage_Project.Services;
using System.Windows;
using System.Linq;

namespace Honours_Stage_Project.ViewModels
{
    public class NodeViewModel : INotifyPropertyChanged
    {
        private readonly INodeConnectionService _connectionService;
        private bool _isDefaultOutgoingVisible = true;
        public ObservableCollection<ConnectionViewModel> ConnectionComponents { get; }
            = new ObservableCollection<ConnectionViewModel>();

        public ObservableCollection<AttributeModel> Attributes => Model.Attributes;
        public NodeModel Model { get; }

        public string TextContent
        {
            get => Model.TextContent;
            set
            {
                if (Model.TextContent == value) return;
                Model.TextContent = value;
                OnPropertyChanged(nameof(TextContent));
            }
        }

        public double X
        {
            get => Model.X;
            set
            {
                if (Model.X == value) return;
                Model.X = value;
                OnPropertyChanged(nameof(X));
            }
        }

        public double Y
        {
            get => Model.Y;
            set
            {
                if (Model.Y == value) return;
                Model.Y = value;
                OnPropertyChanged(nameof(Y));
            }
        }

        public bool IsDefaultOutgoingVisible
        {
            get => _isDefaultOutgoingVisible;
            private set
            {
                if (_isDefaultOutgoingVisible == value) return;
                _isDefaultOutgoingVisible = value;
                OnPropertyChanged(nameof(IsDefaultOutgoingVisible));
            }
        }

        public Size Size
        {
            get => Model.Size;
            set
            {
                if (Model.Size == value) return;
                Model.Size = value;
                OnPropertyChanged(nameof(Size));
            }
        }

        public ICommand AddConnectionComponentCommand { get; }
        public ICommand AddIncomingConnectionCommand { get; }
        public ICommand AddDefaultConnectionCommand { get; }
        public ICommand AddAttributeCommand { get; }


        public NodeViewModel(NodeModel model, INodeConnectionService connectionService)
        {
            Model = model;
            _connectionService = connectionService;

            AddConnectionComponentCommand = new RelayCommand(_ => AddConnectionComponent());
            AddIncomingConnectionCommand = new RelayCommand(_ => _connectionService.AddIncoming(Model.ID));
            AddDefaultConnectionCommand = new RelayCommand(_ => AddDefaultConnection());
            AddAttributeCommand = new RelayCommand(_ => AddAttribute());

            foreach (var component in Model.ConnectionComponents)
            {
                // ID 0 is data-only (default outgoing marker), do not render as a component view.
                if (component.ID == 0)
                    continue;

                ConnectionComponents.Add(new ConnectionViewModel(component, Model.ID, _connectionService));
            }

            if (ConnectionComponents.Count > 0)
                IsDefaultOutgoingVisible = false;
        }

        private void AddConnectionComponent()
        {
            RemoveDefaultConnection();

            var componentModel = Model.AddConnectionComponent();
            ConnectionComponents.Add(new ConnectionViewModel(componentModel, Model.ID, _connectionService));
        }

        private void AddDefaultConnection()
        {
            if (Model.GetComponentConnection(0) != null)
                return;

            _connectionService.AddOutgoing(Model.ID, 0, 0);

            // Keep default connection in model only (no UI component row).
            Model.AddDefaultConnectionComponent();
        }

        private void AddAttribute()
        {
            Attributes.Add(new AttributeModel { Id = Attributes.Count });
        }

        private void RemoveDefaultConnection()
        {
            _connectionService.RemoveOutgoing(Model.ID, 0, 0);
            ConnectionComponents.Remove(ConnectionComponents.FirstOrDefault(c => c.ID == 0));
            Model.RemoveConnectionComponent(0);
            IsDefaultOutgoingVisible = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
