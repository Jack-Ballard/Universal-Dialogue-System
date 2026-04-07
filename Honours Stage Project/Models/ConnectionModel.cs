using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Honours_Stage_Project.Models
{
    public class ConnectionModel
    {
        public int ID { get; set; }

        public List<Point> ButtonPositions { get; set; }

        public ObservableCollection<AttributeModel> Attributes { get; } = new ObservableCollection<AttributeModel>();
        public ObservableCollection<ConditionModel> Conditions { get; } = new ObservableCollection<ConditionModel>();
        public ObservableCollection<OutgoingConnectionModel> OutgoingConnections { get; } = new ObservableCollection<OutgoingConnectionModel>();

        public ConnectionModel(int id)
        {
            ID = id;
            ButtonPositions = new List<Point>();
        }

        public OutgoingConnectionModel AddOutgoingConnection()
        {
            var nextId = OutgoingConnections.Count == 0 ? 0 : OutgoingConnections.Max(c => c.Id) + 1;
            var outgoing = new OutgoingConnectionModel(nextId);
            OutgoingConnections.Add(outgoing);
            return outgoing;
        }

        public object Export()
        {
            var exportedAttributes = new List<object>();
            foreach (var attribute in Attributes)
                exportedAttributes.Add(attribute.Export());

            var exportedOutgoingConnections = new List<object>();
            foreach (var outgoingConnection in OutgoingConnections)
                exportedOutgoingConnections.Add(outgoingConnection.Export());

            var exportedConditions = new List<object>();
                foreach (var condition in Conditions)
                    exportedConditions.Add(condition.Export());

            return new
            {
                ID,
                Attributes = exportedAttributes,
                Conditions = exportedConditions,
                OutgoingConnections = exportedOutgoingConnections
            };
        }

        public void Import(dynamic connectionData)
        {
            ID = connectionData.ID;
            ButtonPositions = new List<Point>();
            Attributes.Clear();
            foreach (var attributeData in connectionData.Attributes)
            {
                var attributeModel = new AttributeModel();
                //{
                //    Id = attributeData.ID,
                //    Name = attributeData.Name,
                //    Value = attributeData.Value
                //};
                attributeModel.Import(attributeData);
                Attributes.Add(attributeModel);
            }
            foreach (var outgoingData in connectionData.OutgoingConnections)
            {
                var outgoingModel = new OutgoingConnectionModel();
                outgoingModel.Import(outgoingData);
                OutgoingConnections.Add(outgoingModel);
            }
            foreach (var conditionData in connectionData.Conditions)
            {
                var conditionModel = new ConditionModel();
                conditionModel.Import(conditionData);
                Conditions.Add(conditionModel);
            }
        }
    }
}
