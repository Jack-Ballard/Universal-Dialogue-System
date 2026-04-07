using Honours_Stage_Project.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Honours_Stage_Project.Helpers;
using Honours_Stage_Project.Services;

namespace Honours_Stage_Project.ViewModels
{
    public class NodeViewModel : INotifyPropertyChanged
    {
        private readonly INodeConnectionService _connectionService;
        private bool _isDefaultOutgoingVisible = true;

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

        public ObservableCollection<ConnectionViewModel> ConnectionComponents { get; }
            = new ObservableCollection<ConnectionViewModel>();

        public ObservableCollection<AttributeModel> Attributes => Model.Attributes;

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
            AddDefaultConnectionCommand = new RelayCommand(_ => _connectionService.AddOutgoing(Model.ID, 0, 0));
            AddAttributeCommand = new RelayCommand(_ => AddAttribute());

            foreach (var component in Model.ConnectionComponents)
                ConnectionComponents.Add(new ConnectionViewModel(component, Model.ID, _connectionService));

            if (ConnectionComponents.Count > 0)
                IsDefaultOutgoingVisible = false;
        }

        private void AddConnectionComponent()
        {
            var componentModel = Model.AddConnectionComponent();
            ConnectionComponents.Add(new ConnectionViewModel(componentModel, Model.ID, _connectionService));

            RemoveDefaultConnection();
        }

        private void AddAttribute()
        {
            Attributes.Add(new AttributeModel { Id = Attributes.Count });
        }

        private void RemoveDefaultConnection()
        {
            _connectionService.RemoveOutgoing(Model.ID, 0, 0);
            IsDefaultOutgoingVisible = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
