namespace Domain.Runtime.WorldDomain
{
    public class ConnectionTopology
    {
        #region Attributes
        private readonly Dictionary<string, RoomConnectionInstance> connectionsById = new();
        private readonly Dictionary<string, RoomConnectionInstance> connectionsBySourceEntity = new();
        #endregion

        #region Properties
        #endregion

        public ConnectionTopology()
        {

        }

        #region Methods
        public void AddConnection(
            RoomConnectionInstance connection)
        {
            connectionsById[connection.ID] = connection;

            connectionsBySourceEntity[
                connection.SourceEntityInstanceID] = connection;
        }

        public void RemoveConnection(
            string connectionId)
        {
            if (!connectionsById.TryGetValue(
                connectionId,
                out var connection))
            {
                return;
            }

            connectionsBySourceEntity.Remove(
                connection.SourceEntityInstanceID);

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