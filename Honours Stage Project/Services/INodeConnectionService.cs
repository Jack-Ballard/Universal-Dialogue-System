using System;
using System.Collections.Generic;

namespace Honours_Stage_Project.Services
{
    public interface INodeConnectionService
    {
        IReadOnlyList<(int, int, int)> Connections { get; }

        event Action ConnectionsChanged;

        void AddOutgoing(int nodeId, int componentId);

        void AddIncoming(int nodeId);

        void RemoveOutgoing(int nodeId, int componentId);
    }
}
