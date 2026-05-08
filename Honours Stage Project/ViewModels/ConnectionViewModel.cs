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
        private readonly ConnectionModel _model;
        private bool _isDefaultOutgoingVisible = true;
        private readonly int _nodeId;

        public int ID => _model.ID;

        public ObservableCollection<AttributeModel> Attributes => _model.Attributes;
        public ObservableCollection<ConditionModel> Conditions => _model.Conditions;
        public ObservableCollection<OutgoingConnectionViewModel> OutgoingConnections { get; }
            = new ObservableCollection<OutgoingConnectionViewModel>();

        //public ObservableCollection<ConditionModel> ConectionConditions
        //    => OutgoingConnections.FirstOrDefault() != null
        //        ? OutgoingConnections.First().Conditions
        //        : new ObservableCollection<ConditionModel>();

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
            _model = model;
            _nodeId = nodeId;
            _connectionService = connectionService;

            AddAttributeCommand = new RelayCommand(_ =>
                _model.Attributes.Add(new AttributeModel { Id = _model.Attributes.Count }));
            RemoveComponentAttributeCommand = new RelayCommand(attribute =>
                RemoveComponentAttribute(attribute as AttributeModel));

            AddConditionCommand = new RelayCommand(_ =>
                _model.Conditions.Add(new ConditionModel { Id = _model.Conditions.Count }));
            RemoveComponentConditionCommand = new RelayCommand(condition =>
                RemoveComponentCondition(condition as ConditionModel));

            AddOutgoingOptionCommand = new RelayCommand(_ => AddOutgoingOption());

            AddDefaultOutgoingCommand = new RelayCommand(_ =>
                _connectionService.AddOutgoing(_nodeId, ID, 0));
            RemoveOutgoingConnectionCommand = new RelayCommand(connection =>
                RemoveOutgoingConnection(connection as OutgoingConnectionViewModel));

            foreach (var outgoing in _model.OutgoingConnections)
            {
                OutgoingConnections.Add(new OutgoingConnectionViewModel(
                    outgoing,
                    _nodeId,
                    _model.ID,
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
            var outgoing = _model.AddOutgoingConnection();
            OutgoingConnections.Add(new OutgoingConnectionViewModel(
                outgoing,
                _nodeId,
                _model.ID,
                _connectionService));

            RemoveDefaultConnection();
        }

        private void RemoveDefaultConnection()
        {
            _connectionService.RemoveOutgoing(_nodeId, _model.ID, 0);
            IsDefaultOutgoingVisible = false;
        }

        private void RemoveComponentAttribute(AttributeModel attribute)
        {
            if (attribute == null)
                return;

            _model.Attributes.Remove(attribute);

            for (int i = 0; i < _model.Attributes.Count; i++)
                _model.Attributes[i].Id = i;
        }

        private void RemoveComponentCondition(ConditionModel condition)
        {
            if (condition == null)
                return;

            _model.Conditions.Remove(condition);

            for (int i = 0; i < _model.Conditions.Count; i++)
                _model.Conditions[i].Id = i;
        }

        private void RemoveOutgoingConnection(OutgoingConnectionViewModel connection)
        {
            if (connection == null)
                return;

            // Remove existing graph edge(s) for this outgoing branch
            _connectionService.RemoveOutgoing(_nodeId, _model.ID, connection.ID);

            // Keep VM and Model collections in sync
            OutgoingConnections.Remove(connection);

            var outgoingModel = _model.OutgoingConnections.FirstOrDefault(o => o.Id == connection.ID);
            if (outgoingModel != null)
                _model.OutgoingConnections.Remove(outgoingModel);

            _connectionService.DecrementConnections(_nodeId, _model.ID, connection.ID);

            // Re-index remaining outgoing branches and update service mappings
            foreach (var outgoing in OutgoingConnections)
            {
                if (outgoing.ID > connection.ID)
                    outgoing.DecrementId();
            }

            IsDefaultOutgoingVisible = OutgoingConnections.Count == 0;
        }
    }
}
