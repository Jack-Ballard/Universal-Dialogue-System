using System;
using System.Collections.Generic;

namespace Honours_Stage_Project.Services
{
    public struct Connection
    {
        public int NodeId { get; }
        public int ComponentId { get; }
        public int ConnectionId { get; }
        public int TargetNodeId { get; }
        public Connection(int nodeId, int componentId, int connectionId, int targetNodeId)
        {
            NodeId = nodeId;
            ComponentId = componentId;
            ConnectionId = connectionId;
            TargetNodeId = targetNodeId;
        }
    }
    public interface INodeConnectionService
    {
        IReadOnlyList<Connection> Connections { get; }

        event Action ConnectionsChanged;

        void AddOutgoing(int nodeId, int componentId, int connectionId);

        void AddIncoming(int nodeId);

        void RemoveOutgoing(int nodeId, int componentId, int connectionId);

        void RemoveConnectionsForNode(int nodeId);

        void SetConnections(IEnumerable<Connection> connections);
    }
}
