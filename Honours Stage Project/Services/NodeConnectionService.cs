using System;
using System.Collections.Generic;

namespace Honours_Stage_Project.Services
{
    public class NodeConnectionService : INodeConnectionService
    {
        private readonly List<(int, int, int)> _connections = new List<(int, int, int)>();
        private (int nodeId, int componentId) _pendingOutgoing = (-1, -1);
        private int _pendingIncoming = -1;

        public IReadOnlyList<(int, int, int)> Connections => _connections;

        public event Action ConnectionsChanged;

        public void AddOutgoing(int nodeId, int componentId)
        {
            if(_connections.Exists(c => c.Item1 == nodeId && c.Item2 == componentId))
            {
                RemoveOutgoing(nodeId, componentId);
                return;
            }

            _pendingOutgoing = (nodeId, componentId);
            if (_pendingIncoming != -1)
                CommitConnection();
        }

        public void AddIncoming(int nodeId)
        {
            _pendingIncoming = nodeId;
            if (_pendingOutgoing != (-1, -1))
                CommitConnection();
        }

        private void CommitConnection()
        {
            _connections.Add((_pendingOutgoing.nodeId, _pendingOutgoing.componentId, _pendingIncoming));
            _pendingOutgoing = (-1, -1);
            _pendingIncoming = -1;
            ConnectionsChanged?.Invoke();
        }

        public void RemoveOutgoing(int nodeId, int componentId)
        {
            _connections.RemoveAll(c => c.Item1 == nodeId && c.Item2 == componentId);
            ConnectionsChanged?.Invoke();
        }

        public void SetConnections(IEnumerable<(int, int, int)> connections)
        {
            _connections.Clear();
            _connections.AddRange(connections);
            ConnectionsChanged?.Invoke();
        }

    }
}
