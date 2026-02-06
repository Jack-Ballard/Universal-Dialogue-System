using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace Honours_Stage_Project.Node
{
    public class TextBoxModel
    {
        public string TextContent;
        public int ID;
        //private List<ComponentConnection> _connectionComponents = new List<ComponentConnection>();
        public TextBoxModel(int id) 
        {
            ID = id;
        }

        public ObservableCollection<ComponentConnection> ConnectionComponents { get; } = new ObservableCollection<ComponentConnection>();

        public ComponentConnection AddConnectionComponent()
        {
            ComponentConnection componentConnection = new ComponentConnection(ConnectionComponents.Count());
            ConnectionComponents.Add(componentConnection);
            return componentConnection;
        }
    }
}
