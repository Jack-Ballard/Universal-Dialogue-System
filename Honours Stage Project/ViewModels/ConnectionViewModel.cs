using Honours_Stage_Project.Helpers;
using Honours_Stage_Project.Models;
using Honours_Stage_Project.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Honours_Stage_Project.ViewModels
{
    public class ConnectionViewModel : INotifyPropertyChanged
    {
        private readonly INodeConnectionService _connectionService;
        private bool _isDefaultOutgoingVisible = true;
        private readonly int _nodeId;

        public ConnectionModel Model { get; }

        public int ID => Model.ID;

        public ObservableCollection<AttributeModel> Attributes => Model.Attributes;
        public ObservableCollection<ConditionModel> Conditions => Model.Conditions;
        public ObservableCollection<OutgoingConnectionViewModel> OutgoingConnections { get; }
            = new ObservableCollection<OutgoingConnectionViewModel>();

        public ObservableCollection<ConditionModel> ConectionConditions
            => OutgoingConnections.FirstOrDefault() != null
                ? OutgoingConnections.First().Conditions
                : new ObservableCollection<ConditionModel>();

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

        public ICommand AddAttributeCommand { get; }
        public ICommand RemoveComponentAttributeCommand { get; }
        public ICommand AddConditionCommand { get; }
        public ICommand RemoveComponentConditionCommand { get; }
        public ICommand AddOutgoingOptionCommand { get; }
        public ICommand AddDefaultOutgoingCommand { get; }
        public ICommand RemoveOutgoingConnectionCommand { get; }

        public ConnectionViewModel(ConnectionModel model, int nodeId, INodeConnectionService connectionService)
        {
            Model = model;
            _nodeId = nodeId;
            _connectionService = connectionService;

            AddAttributeCommand = new RelayCommand(_ =>
                Model.Attributes.Add(new AttributeModel { Id = Model.Attributes.Count }));
            RemoveComponentAttributeCommand = new RelayCommand(attribute =>
                RemoveComponentAttribute(attribute as AttributeModel));

            AddConditionCommand = new RelayCommand(_ =>
                Model.Conditions.Add(new ConditionModel { Id = Model.Conditions.Count }));
            RemoveComponentConditionCommand = new RelayCommand(condition =>
                RemoveComponentCondition(condition as ConditionModel));

            AddOutgoingOptionCommand = new RelayCommand(_ => AddOutgoingOption());

            AddDefaultOutgoingCommand = new RelayCommand(_ =>
                _connectionService.AddOutgoing(_nodeId, ID, 0));
            RemoveOutgoingConnectionCommand = new RelayCommand(connection => RemoveOutgoingConnection(connection as OutgoingConnectionViewModel));

            foreach (var outgoing in Model.OutgoingConnections)
            {
                OutgoingConnections.Add(new OutgoingConnectionViewModel(
                    outgoing,
                    _nodeId,
                    Model.ID,
                    _connectionService));
            }

            // Hide default button if component already has additional outgoing branches
            if (OutgoingConnections.Count > 0)
                IsDefaultOutgoingVisible = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void AddOutgoingOption()
        {
            var outgoing = Model.AddOutgoingConnection();
            OutgoingConnections.Add(new OutgoingConnectionViewModel(
                outgoing,
                _nodeId,
                Model.ID,
                _connectionService));

            RemoveDefaultConnection();
        }

        private void RemoveDefaultConnection()
        {
            _connectionService.RemoveOutgoing(_nodeId, Model.ID, 0);
            IsDefaultOutgoingVisible = false;
        }

        private void RemoveComponentAttribute(AttributeModel attribute)
        {
            if (attribute == null)
                return;

            Model.Attributes.Remove(attribute);

            for (int i = 0; i < Model.Attributes.Count; i++)
                Model.Attributes[i].Id = i;
        }

        private void RemoveComponentCondition(ConditionModel condition)
        {
            if (condition == null)
                return;

            Model.Conditions.Remove(condition);

            for (int i = 0; i < Model.Conditions.Count; i++)
                Model.Conditions[i].Id = i;
        }

        private void RemoveOutgoingConnection(OutgoingConnectionViewModel connection)
        {
            if (connection == null)
                return;

            // Remove existing graph edge(s) for this outgoing branch
            _connectionService.RemoveOutgoing(_nodeId, Model.ID, connection.ID);

            // Keep VM and Model collections in sync
            OutgoingConnections.Remove(connection);
            Model.OutgoingConnections.Remove(connection.Model);
            _connectionService.DecrementConnections(_nodeId, Model.ID, connection.ID);

            // Re-index remaining outgoing branches and update service mappings
            foreach (var outgoing in OutgoingConnections)
            {
                if (outgoing.ID > connection.ID)
                {
                    outgoing.Model.Id--;
                }
            }

            IsDefaultOutgoingVisible = OutgoingConnections.Count == 0;
        }
    }
}
