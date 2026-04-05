using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Honours_Stage_Project.Models
{
    public class ConnectionModel
    {
        public int ID { get; set; }

        public ObservableCollection<AttributeModel> Attributes { get; } = new ObservableCollection<AttributeModel>();

        public List<string> Conditions { get; } = new List<string>();

        public ConnectionModel(int id)
        {
            ID = id;
        }

        public object Export()
        {
            var exportedAttributes = new List<object>();
            foreach (var attribute in Attributes)
                exportedAttributes.Add(attribute.Export());
            return new { ID, Attributes = exportedAttributes, Conditions };
        }
    }
}
