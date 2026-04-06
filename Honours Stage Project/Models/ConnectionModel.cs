using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Honours_Stage_Project.Models
{
    public class ConnectionModel
    {
        public int ID { get; set; }

        public List<Point> ButtonPositions { get; set; }

        public ObservableCollection<AttributeModel> Attributes { get; } = new ObservableCollection<AttributeModel>();
        public ObservableCollection<ConditionModel> Conditions { get; } = new ObservableCollection<ConditionModel>();

        public ConnectionModel(int id)
        {
            ID = id;
            ButtonPositions = new List<Point>();
        }

        public object Export()
        {
            var exportedAttributes = new List<object>();
            foreach (var attribute in Attributes)
                exportedAttributes.Add(attribute.Export());

            var exportedConditions = new List<object>();
            foreach (var condition in Conditions)
                exportedConditions.Add(condition.Export());

            return new { ID, Attributes = exportedAttributes, Conditions = exportedConditions };
        }

        public void Import(object data)
        {
            var connectionData = (Newtonsoft.Json.Linq.JObject)data;
            ID = (int)connectionData["ID"];
            Attributes.Clear();
            foreach (var attribute in connectionData["Attributes"])
            {
                var attributeModel = new AttributeModel(); // ID will be set in Import
                attributeModel.Import(attribute);
                Attributes.Add(attributeModel);
            }
            Conditions.Clear();
            foreach (var condition in connectionData["Conditions"])
            {
                var conditionModel = new ConditionModel(); // ID will be set in Import
                conditionModel.Import(condition);
                Conditions.Add(conditionModel);
            }

        }
    }
}
