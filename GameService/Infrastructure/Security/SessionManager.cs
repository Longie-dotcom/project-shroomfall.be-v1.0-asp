using Application.Interfaces.Security;
using System.Collections.Concurrent;

namespace Infrastructure.Security
{
    public class SessionManager : ISessionManager
    {
        #region Attributes
        private readonly ConcurrentDictionary<string, string> userToPlayer = new();
        private readonly ConcurrentDictionary<string, string> playerToUser = new();
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
            if (userToPlayer.TryRemove(userId, out var oldPlayerId))
            {
                playerToUser.TryRemove(oldPlayerId, out _);
            }

            userToPlayer[userId] = playerInstanceId;
            playerToUser[playerInstanceId] = userId;
        }

        public string? Remove(
            string userId)
        {
            if (userToPlayer.TryRemove(userId, out var playerInstanceId))
            {
                playerToUser.TryRemove(playerInstanceId, out _);
                return playerInstanceId;
            }
            return null;
        }

        public string? Get(
            string userId)
        {
            return userToPlayer.TryGetValue(userId, out var playerInstanceId)
                ? playerInstanceId
                : null;
        }

        public string? GetUserIdByPlayerId(
            string playerInstanceId)
        {
            return playerToUser.TryGetValue(playerInstanceId, out var userId)
                ? userId
                : null;
        }
        #endregion
    }
}