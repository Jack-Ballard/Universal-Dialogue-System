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
        private static List<(TextBoxViewModel, int, Button, TextBoxViewModel)> connections = new List<(TextBoxViewModel, int, Button, TextBoxViewModel)>();
        private static (TextBoxViewModel, int, Button) incommingConnection;
        private static TextBoxViewModel outgoingConnection;

        public static void AddOutgoingConnection(TextBoxViewModel textBoxViewModel, int componentConnection, Button button)
        {
            incommingConnection = (textBoxViewModel, componentConnection, button);
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
            connections.Add((incommingConnection.Item1, incommingConnection.Item2, incommingConnection.Item3, outgoingConnection));
            incommingConnection = (null, -1, null);
            outgoingConnection = null;
        }

        public static List<(TextBoxViewModel, int, Button, TextBoxViewModel)> GetConnections()
        {
            return connections;
        }
    }
}
