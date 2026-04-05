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

        public List<string> Conditions { get; } = new List<string>();

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
            return new { ID, Attributes = exportedAttributes, Conditions };
        }
    }
}
