using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Honours_Stage_Project.Node;

namespace Honours_Stage_Project
{
    public static class NodeConnections
    {
        private static List<(TextBoxViewModel, int, TextBoxViewModel)> connections = new List<(TextBoxViewModel, int, TextBoxViewModel)>();
        private static (TextBoxViewModel, int) incommingConnection;
        private static TextBoxViewModel outgoingConnection;

        public static void AddOutgoingConnection(TextBoxViewModel textBoxViewModel, int componentConnection)
        {
            incommingConnection = (textBoxViewModel, componentConnection);
            if(outgoingConnection != null)
            {
                CommitConnection();
            }
        }
        public static void AddIncommingConnection(TextBoxViewModel textBoxViewModel)
        {
            outgoingConnection = textBoxViewModel;
            if (incommingConnection.Item1 != null)
            {
                CommitConnection();
            }
        }
        private static void CommitConnection()
        {
            connections.Add((incommingConnection.Item1, incommingConnection.Item2, outgoingConnection));
            incommingConnection = (null, -1);
            outgoingConnection = null;
        }

        public static List<(TextBoxViewModel, int, TextBoxViewModel)> GetConnections()
        {
            return connections;
        }
    }
}
