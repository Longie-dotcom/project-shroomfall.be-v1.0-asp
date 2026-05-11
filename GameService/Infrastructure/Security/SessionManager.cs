using Application.Interfaces.Security;
using System.Collections.Concurrent;

namespace Infrastructure.Security
{
    public class SessionManager : ISessionManager
    {
        #region Attributes
        private readonly ConcurrentDictionary<string, string> userToPlayer = new();
        #endregion

        #region Properties
        #endregion

        public SessionManager()
        {

        }

        #region Methods
        public void Add(
            string userId, 
            string playerInstanceId)
        {
            userToPlayer[userId] = playerInstanceId;
        }

        public string? Remove(
            string userId)
        {
            return userToPlayer.TryRemove(userId, out var playerInstanceId)
                ? playerInstanceId
                : null;
        }

        public string? Get(
            string userId)
        {
            return userToPlayer.TryGetValue(userId, out var playerInstanceId)
                ? playerInstanceId
                : null;
        }
        #endregion
    }
}