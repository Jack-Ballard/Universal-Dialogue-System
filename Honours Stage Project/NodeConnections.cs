using Honours_Stage_Project.Node;
using System.Collections.Generic;

namespace Honours_Stage_Project
{
    public static class NodeConnections
    {
        private static List<(TextBoxViewModel, ComponentConnection, TextBoxViewModel)> connections = new List<(TextBoxViewModel, ComponentConnection, TextBoxViewModel)>();
        private static (TextBoxViewModel, ComponentConnection) outgoingConnection;
        private static TextBoxViewModel incomingConnection; 

        public static void AddOutgoingConnection(TextBoxViewModel textBoxViewModel, ComponentConnection componentConnection)
        {
            outgoingConnection = (textBoxViewModel, componentConnection);
            if(incomingConnection != null)
            {
                CommitConnection();
            }
        }
        public static void AddIncomingConnection(TextBoxViewModel textBoxViewModel)
        {
            incomingConnection = textBoxViewModel;
            if (outgoingConnection.Item1 != null)
            {
                CommitConnection();
            }
        }
        private static void CommitConnection()
        {
            connections.Add((outgoingConnection.Item1, outgoingConnection.Item2, incomingConnection));
            outgoingConnection = (null, null);
            incomingConnection = null;
        }

        public static List<(TextBoxViewModel, ComponentConnection, TextBoxViewModel)> GetConnections()
        {
            return connections;
        }

        public static List<(int, int, int)> GetConnectionIDs()
        {
            List<(int, int, int)> connectionIDs = new List<(int, int, int)>();
            foreach (var connection in connections)
            {
                int fromNodeID = connection.Item1.GetTextBoxModel().ID;
                int componentConnectionID = connection.Item2.ID;
                int toNodeID = connection.Item3.GetTextBoxModel().ID;
                connectionIDs.Add((fromNodeID, componentConnectionID, toNodeID));
            }
            return connectionIDs;
        }
    }
}
