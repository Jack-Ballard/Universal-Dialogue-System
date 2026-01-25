using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace Honours_Stage_Project.Node
{
    public class TextBoxModel
    {
        public TextBoxModel() { }
        public string TextContent;
        private List<ComponentConnection> _connectionComponents = new List<ComponentConnection>();

        public ComponentConnection AddConnectionComponent()
        {
            ComponentConnection componentConnection = new ComponentConnection(_connectionComponents.Count());
            _connectionComponents.Add(componentConnection);
            return componentConnection;
        }
    }
}
