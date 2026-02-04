using Honours_Stage_Project.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Honours_Stage_Project.Node
{
    public class ComponentConnection
    {
        public int ID { get; set; }
        private List<string> _attributes = new List<string>();
        private List<string> _conditions = new List<string>();
        public ICommand AddAttributeCommand { get; set; }
        public ICommand AddConditionCommand { get; set; }

        public ComponentConnection(int id)
        {
            ID = id;
            AddAttributeCommand = new RelayCommand(AddAttribute);
            AddConditionCommand = new RelayCommand(AddCondition);
        }

        private void AddAttribute(object parameter)
        {
            // Your logic to add an attribute
        }
        private void AddCondition(object parameter)
        {
            // Your logic to add an attribute
        }
    }
}
