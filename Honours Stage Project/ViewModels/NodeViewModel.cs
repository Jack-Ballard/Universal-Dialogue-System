using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Honours_Stage_Project.Helpers;
using Honours_Stage_Project.Models;
using Honours_Stage_Project.Services;

namespace Honours_Stage_Project.ViewModels
{
    public class NodeViewModel : INotifyPropertyChanged
    {
        private readonly INodeConnectionService _connectionService;

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

        public ObservableCollection<ConnectionViewModel> ConnectionComponents { get; }
            = new ObservableCollection<ConnectionViewModel>();

        public ICommand AddConnectionComponentCommand { get; }
        public ICommand AddIncomingConnectionCommand { get; }

        public NodeViewModel(NodeModel model, INodeConnectionService connectionService)
        {
            Model = model;
            _connectionService = connectionService;

            AddConnectionComponentCommand = new RelayCommand(_ => AddConnectionComponent());
            AddIncomingConnectionCommand = new RelayCommand(_ => _connectionService.AddIncoming(Model.ID));
        }

        private void AddConnectionComponent()
        {
            var componentModel = Model.AddConnectionComponent();
            ConnectionComponents.Add(new ConnectionViewModel(componentModel, Model.ID, _connectionService));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
