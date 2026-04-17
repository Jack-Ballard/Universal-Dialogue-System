using System;
using System.Collections.Generic;

namespace Honours_Stage_Project.Services
{
    public class NodeConnectionService : INodeConnectionService
    {
        private readonly List<Connection> _connections = new List<Connection>();
        private (int nodeId, int componentId, int connectionId) _pendingOutgoing = (-1, -1, -1);

        public (int NodeId, int ComponentId, int ConnectionId) PendingOutgoing => _pendingOutgoing;

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
        }

        public void AddIncoming(int nodeId)
        {
            if (_pendingOutgoing != (-1, -1, -1))
                CommitConnection(nodeId);
        }

        public void RemoveConnectionComponent(int nodeId, int componentId)
        {
            List<Connection> connectionsToUpdate = _connections.FindAll(c => c.NodeId == nodeId && c.ComponentId > componentId);
            _connections.RemoveAll(c => c.NodeId == nodeId && c.ComponentId > componentId);

            for (int i = 0; i < connectionsToUpdate.Count; i++)
            {
                Connection c = connectionsToUpdate[i];
                _connections.Add(new Connection(c.NodeId, c.ComponentId - 1, c.ConnectionId, c.TargetNodeId));
            }

            ConnectionsChanged?.Invoke();
        }

        public void DecrementConnections(int NodeId, int ComponentId, int ConnectionId)
        {
            List<Connection> connectionsToUpdate = _connections.FindAll(c => c.NodeId == NodeId && c.ComponentId == ComponentId && c.ConnectionId > ConnectionId);
            _connections.RemoveAll(c => c.NodeId == NodeId && c.ComponentId == ComponentId && c.ConnectionId > ConnectionId);
            for (int i = 0; i < connectionsToUpdate.Count; i++)
            {
                Connection c = connectionsToUpdate[i];
                _connections.Add(new Connection(c.NodeId, c.ComponentId, c.ConnectionId - 1, c.TargetNodeId));
            }
            ConnectionsChanged?.Invoke();
        }

        private void CommitConnection(int incomingNodeId)
        {
            Connection connection = new Connection(_pendingOutgoing.nodeId, _pendingOutgoing.componentId, _pendingOutgoing.connectionId, incomingNodeId);
            _connections.Add(connection);
            _pendingOutgoing = (-1, -1, -1);
            ConnectionsChanged?.Invoke();
        }

        public void RemoveOutgoing(int nodeId, int componentId, int connectionId)
        {
            _connections.RemoveAll(c => c.NodeId == nodeId && c.ComponentId == componentId && c.ConnectionId == connectionId);
            ConnectionsChanged?.Invoke();
        }

        public void RemoveIncoming(int nodeId)
        {
            Connection pending = _connections.Find(c => c.TargetNodeId == nodeId);
            _connections.RemoveAll(c => c.TargetNodeId == nodeId);
            //_pendingOutgoing = (pending.NodeId, pending.ComponentId, pending.ConnectionId);
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
