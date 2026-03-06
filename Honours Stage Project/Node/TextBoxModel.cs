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
        public ObservableCollection<ComponentConnection> ConnectionComponents { get; } = new ObservableCollection<ComponentConnection>();
        public TextBoxModel(int id) 
        {
            ID = id;
        }

        public ComponentConnection AddConnectionComponent()
        {
            ComponentConnection componentConnection = new ComponentConnection(ConnectionComponents.Count());
            ConnectionComponents.Add(componentConnection);
            return componentConnection;
        }

        public ComponentConnection GetComponentConnection(int id)
        {
            return ConnectionComponents.FirstOrDefault(c => c.ID == id);
        }
        
        public Object Export()
        {
            List<Object> exportedConnections = new List<Object>();
            foreach (ComponentConnection connection in ConnectionComponents)
            {
                exportedConnections.Add(connection.Export());
            }
            return new
            {
                ID = ID,
                TextContent = TextContent,
                Connections = exportedConnections
            };
        }
    }
}
