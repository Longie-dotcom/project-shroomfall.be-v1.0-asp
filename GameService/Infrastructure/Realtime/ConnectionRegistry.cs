using Application.Interfaces.Realtime;
using System.Collections.Concurrent;

namespace Infrastructure.Realtime
{
    public class ConnectionRegistry : IConnectionRegistry
    {
        #region Attributes
        private readonly ConcurrentDictionary<string, string> userToConnection = new();
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
            userToConnection[userId] = connectionId;
        }

        public string? Remove(
            string userId)
        {
            return userToConnection.TryRemove(userId, out var connection)
                ? connection
                : null;
        }

        public string? Get(
            string userId)
        {
            return userToConnection.TryGetValue(userId, out var connection)
                ? connection
                : null;
        }
        #endregion
    }
}