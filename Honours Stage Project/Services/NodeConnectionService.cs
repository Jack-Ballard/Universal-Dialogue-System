using System;
using System.Collections.Generic;

namespace Honours_Stage_Project.Services
{
    public class NodeConnectionService : INodeConnectionService
    {
        private readonly List<Connection> _connections = new List<Connection>();
        private (int nodeId, int componentId, int connectionId) _pendingOutgoing = (-1, -1, -1);
        private int _pendingIncoming = -1;

        public IReadOnlyList<Connection> Connections => _connections;

        public event Action ConnectionsChanged;

        public void AddOutgoing(int nodeId, int componentId, int connectionId)
        {
            if (_connections.Exists(c => c.NodeId == nodeId && c.ComponentId == componentId && c.ConnectionId == connectionId))
            {
                RemoveOutgoing(nodeId, componentId, connectionId);
                return;
            }

            _pendingOutgoing = (nodeId, componentId, connectionId);
            if (_pendingIncoming != -1)
                CommitConnection();
        }

        public void AddIncoming(int nodeId)
        {
            _pendingIncoming = nodeId;
            if (_pendingOutgoing != (-1, -1, -1))
                CommitConnection();
        }

        private void CommitConnection()
        {
            Connection connection = new Connection(_pendingOutgoing.nodeId, _pendingOutgoing.componentId, _pendingOutgoing.connectionId, _pendingIncoming);
            _connections.Add(connection);
            _pendingOutgoing = (-1, -1, -1);
            _pendingIncoming = -1;
            ConnectionsChanged?.Invoke();
        }

        public void RemoveOutgoing(int nodeId, int componentId, int connectionId)
        {
            _connections.RemoveAll(c => c.NodeId == nodeId && c.ComponentId == componentId && c.ConnectionId == connectionId);
            ConnectionsChanged?.Invoke();
        }

        public void RemoveConnectionsForNode(int nodeId)
        {
            _connections.RemoveAll(c => c.NodeId == nodeId || c.TargetNodeId == nodeId);

            // Create a new list with updated NodeId values for nodes after the removed node
            for (int i = 0; i < _connections.Count; i++)
            {
                Connection c = _connections[i];
                if(c.NodeId > nodeId && c.TargetNodeId > nodeId)
                {
                    _connections[i] = new Connection(c.NodeId - 1, c.ComponentId, c.ConnectionId, c.TargetNodeId - 1);
                }
                else if (c.NodeId > nodeId)
                {
                    _connections[i] = new Connection(c.NodeId - 1, c.ComponentId, c.ConnectionId, c.TargetNodeId);
                }
                else if (c.TargetNodeId > nodeId)
                {
                    _connections[i] = new Connection(c.NodeId, c.ComponentId, c.ConnectionId, c.TargetNodeId - 1);
                }
            }

            ConnectionsChanged?.Invoke();
        }

        public void SetConnections(IEnumerable<Connection> connections)
        {
            _connections.Clear();
            _connections.AddRange(connections);
            ConnectionsChanged?.Invoke();
        }

        public List<Connection> GetConnectionsForNode(int nodeId)
        {
            return _connections.FindAll(c => c.NodeId == nodeId || c.TargetNodeId == nodeId);
        }
    }
}
