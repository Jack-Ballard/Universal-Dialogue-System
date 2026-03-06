using Honours_Stage_Project.Node;
using System;
using System.Collections.Generic;

namespace Honours_Stage_Project
{
    public static class NodeConnections
    {
        private static List<(int, int, int)> connections = new List<(int, int, int)>();
        private static (int, int) outgoingConnection = (-1,-1);
        private static int incomingConnection = -1; 

        public static void AddOutgoingConnection(int textBoxViewModel, int componentConnection)
        {
            outgoingConnection = (textBoxViewModel, componentConnection);
            if(incomingConnection != -1)
            {
                CommitConnection();
            }
        }
        public static void AddIncomingConnection(int textBoxViewModel)
        {
            incomingConnection = textBoxViewModel;
            if (outgoingConnection != (-1, -1))
            {
                CommitConnection();
            }
        }
        private static void CommitConnection()
        {
            connections.Add((outgoingConnection.Item1, outgoingConnection.Item2, incomingConnection));
            outgoingConnection = (-1, -1);
            incomingConnection = -1;
        }

        public static List<(int, int, int)> GetConnections()
        {
            return connections;
        }
        public static List<Object> GetConnectionsObject()
        {
            List<Object> connectionsPackage = new List<Object>();
            foreach ((int, int, int) connection in connections)
            {
                object connectionObject = new
                {
                    FromTextBoxID = connection.Item1,
                    FromComponentID = connection.Item2,
                    ToTextBoxID = connection.Item3
                };
                connectionsPackage.Add(connectionObject);
            }
            return connectionsPackage;
        }
    }
}
