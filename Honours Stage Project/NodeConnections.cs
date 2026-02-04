using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Honours_Stage_Project.Node;

namespace Honours_Stage_Project
{
    public static class NodeConnections
    {
        private static List<(TextBoxViewModel, ComponentConnection, TextBoxViewModel)> connections = new List<(TextBoxViewModel, ComponentConnection, TextBoxViewModel)>();
        private static (TextBoxViewModel, ComponentConnection) outgoingConnection;
        private static TextBoxViewModel incommingConnection; 

        public static void AddOutgoingConnection(TextBoxViewModel textBoxViewModel, ComponentConnection componentConnection)
        {
            outgoingConnection = (textBoxViewModel, componentConnection);
            if(incommingConnection != null)
            {
                CommitConnection();
            }
        }
        public static void AddIncommingConnection(TextBoxViewModel textBoxViewModel)
        {
            incommingConnection = textBoxViewModel;
            if (outgoingConnection.Item1 != null)
            {
                CommitConnection();
            }
        }
        private static void CommitConnection()
        {
            connections.Add((outgoingConnection.Item1, outgoingConnection.Item2, incommingConnection));
            outgoingConnection = (null, null);
            incommingConnection = null;
        }

        public static List<(TextBoxViewModel, ComponentConnection, TextBoxViewModel)> GetConnections()
        {
            return connections;
        }
    }
}
