using System.Collections.ObjectModel;
using System.Windows.Input;
using Honours_Stage_Project.Helpers;
using Honours_Stage_Project.Models;
using Honours_Stage_Project.Services;

namespace Honours_Stage_Project.ViewModels
{
    public class OutgoingConnectionViewModel
    {
        private readonly INodeConnectionService _connectionService;
        private readonly int _nodeId;
        private readonly int _componentId;
        private readonly OutgoingConnectionModel _model;

        public int ID => _model.Id;

        public ObservableCollection<ConditionModel> Conditions => _model.Conditions;

        public ICommand AddConditionCommand { get; }
        public ICommand AddOutgoingConnectionCommand { get; }

        public OutgoingConnectionViewModel(
            OutgoingConnectionModel model,
            int nodeId,
            int componentId,
            INodeConnectionService connectionService)
        {
            _model = model;
            _nodeId = nodeId;
            _componentId = componentId;
            _connectionService = connectionService;

            AddConditionCommand = new RelayCommand(_ =>
                _model.Conditions.Add(new ConditionModel { Id = _model.Conditions.Count }));

            AddOutgoingConnectionCommand = new RelayCommand(_ =>
                _connectionService.AddOutgoing(_nodeId, _componentId, _model.Id));
        }

        public void DecrementId()
        {
            _model.Id--;
        }
    }
}