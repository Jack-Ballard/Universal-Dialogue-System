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

        public OutgoingConnectionModel Model { get; }

        public int ID => Model.Id;

        public ObservableCollection<ConditionModel> Conditions => Model.Conditions;

        public ICommand AddConditionCommand { get; }
        public ICommand AddOutgoingConnectionCommand { get; }

        public OutgoingConnectionViewModel(
            OutgoingConnectionModel model,
            int nodeId,
            int componentId,
            INodeConnectionService connectionService)
        {
            Model = model;
            _nodeId = nodeId;
            _componentId = componentId;
            _connectionService = connectionService;

            AddConditionCommand = new RelayCommand(_ =>
                Model.Conditions.Add(new ConditionModel { Id = Model.Conditions.Count }));

            AddOutgoingConnectionCommand = new RelayCommand(_ =>
                _connectionService.AddOutgoing(_nodeId, _componentId, Model.Id));
        }
    }
}