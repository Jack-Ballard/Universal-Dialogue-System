using Honours_Stage_Project.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Honours_Stage_Project.Node
{
    public class ComponentConnection
    {
        public int ID { get; set; }

        // Attribute 
        public ObservableCollection<AttributeItem> Attributes { get; set; } = new ObservableCollection<AttributeItem>();
        public ICommand AddAttributeCommand { get; set; }

        // Condition
        private List<string> _conditions = new List<string>();
        public ICommand AddConditionCommand { get; set; }


        public ComponentConnection(int id)
        {
            ID = id;
            AddAttributeCommand = new RelayCommand(AddAttribute);
            AddConditionCommand = new RelayCommand(AddCondition);
        }

        private void AddAttribute(object parameter)
        {
            AttributeItem attribute = new AttributeItem { Id = Attributes.Count, Value = string.Empty };
            Attributes.Add(attribute);
        }

        private void AddCondition(object parameter)
        {
            // Your logic to add an attribute
            Dictionary<string, string> condition = new Dictionary<string, string>
            {
                { "Condition", "" }
            };
            condition.Add("test", "test");
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var firstAttribute = Attributes[0]; // Dictionary<string, string>
            //var value = firstAttribute["YourKey"];
        }

        public Object Export()
        {
            List<Object> exportedAttributes = new List<Object>();
            foreach (AttributeItem attribute in Attributes)
            {
                exportedAttributes.Add(attribute.Export());
            }
            return new
            {
                ID = ID,
                Attributes = exportedAttributes,
                Conditions = _conditions
            };
        }
    }
}
