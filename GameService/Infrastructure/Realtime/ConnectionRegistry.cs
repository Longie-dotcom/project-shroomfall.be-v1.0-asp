using Application.Interfaces.Realtime;
using System.Collections.Concurrent;

namespace Infrastructure.Realtime
{
    public class ConnectionRegistry : IConnectionRegistry
    {
        #region Attributes
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> userToConnection = new();
        #endregion

        #region Properties
        #endregion

        public ConnectionRegistry()
        {

        }

        #region Methods
        public void Add(
            string userId,
            string connectionId)
        {
            var connections = userToConnection.GetOrAdd(
                userId,
                _ => new ConcurrentDictionary<string, byte>());

            connections[connectionId] = 0;
        }

        public void Remove(
            string userId,
            string connectionId)
        {
            if (!userToConnection.TryGetValue(userId, out var connections))
                return;

            connections.TryRemove(connectionId, out _);

            if (connections.IsEmpty)
            {
                userToConnection.TryRemove(userId, out _);
            }
        }

        public IReadOnlyCollection<string> Get(
            string userId)
        {
            if (!userToConnection.TryGetValue(userId, out var connections))
                return Array.Empty<string>();

            return connections.Keys.ToList();
        }

        public bool HasConnections(
            string userId)
        {
            return userToConnection.TryGetValue(userId, out var connections)
                && !connections.IsEmpty;
        }
        #endregion
    }
}