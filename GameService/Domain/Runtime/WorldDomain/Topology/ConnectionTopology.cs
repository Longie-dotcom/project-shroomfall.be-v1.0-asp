using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;

namespace Domain.Runtime.WorldDomain.Topology
{
    public class ConnectionTopology
    {
        #region Attributes
        private readonly Dictionary<string, RoomConnectionInstance> connectionsById = new();
        private readonly Dictionary<string, RoomConnectionInstance> connectionsBySourceEntity = new();
        #endregion

        #region Properties
        #endregion

        public ConnectionTopology() { }

        #region Methods
        public void AddConnection(
            RoomConnectionInstance connection)
        {
            if (connectionsById.ContainsKey(connection.ID))
                throw new InternalException(
                    DomainCode.ConnectionTopologyCode.DuplicateConnectionId,
                    $"Connection cannot be added. ID '{connection.ID}' already exists in topology.");

            if (connectionsBySourceEntity.ContainsKey(connection.SourceEntityInstanceID))
                throw new InternalException(
                    DomainCode.ConnectionTopologyCode.SourceEntityAlreadyBound,
                    $"Connection cannot be added. Source entity '{connection.SourceEntityInstanceID}' is already bound to another connection.");

            connectionsById[connection.ID] = connection;
            connectionsBySourceEntity[connection.SourceEntityInstanceID] = connection;
        }

        public void RemoveConnection(
            string connectionId)
        {
            if (!connectionsById.TryGetValue(connectionId, out var connection))
                throw new InternalException(
                    DomainCode.ConnectionTopologyCode.ConnectionNotFoundOnRemoved,
                    $"Connection cannot be removed. ID '{connectionId}' was not found in topology.");

            connectionsBySourceEntity.Remove(connection.SourceEntityInstanceID);
            connectionsById.Remove(connectionId);
        }

        public RoomConnectionInstance? GetConnectionByEntityInstanceID(
            string entityInstanceId)
        {
            return connectionsBySourceEntity.TryGetValue(
                entityInstanceId,
                out var connection)
                ? connection
                : null;
        }

        public IEnumerable<RoomConnectionInstance> GetAll()
        {
            return connectionsById.Values;
        }
        #endregion
    }
}