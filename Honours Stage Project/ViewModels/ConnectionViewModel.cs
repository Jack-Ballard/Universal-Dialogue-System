using System.Collections.ObjectModel;
using System.Windows.Input;
using Honours_Stage_Project.Helpers;
using Honours_Stage_Project.Models;
using Honours_Stage_Project.Services;

namespace Honours_Stage_Project.ViewModels
{
    public class ConnectionViewModel
    {
        private readonly INodeConnectionService _connectionService;

        public ConnectionModel Model { get; }

        public int ID => Model.ID;

        public ObservableCollection<AttributeModel> Attributes => Model.Attributes;

        public ICommand AddAttributeCommand { get; }
        public ICommand AddConditionCommand { get; }
        public ICommand AddOutgoingConnectionCommand { get; }

        public ConnectionViewModel(ConnectionModel model, int nodeId, INodeConnectionService connectionService)
        {
            Model = model;
            _connectionService = connectionService;

            AddAttributeCommand = new RelayCommand(_ =>
                Model.Attributes.Add(new AttributeModel { Id = Model.Attributes.Count }));

            AddConditionCommand = new RelayCommand(_ =>
                Model.Conditions.Add(string.Empty));

            AddOutgoingConnectionCommand = new RelayCommand(_ =>
                _connectionService.AddOutgoing(nodeId, Model.ID));
        }
    }
}
