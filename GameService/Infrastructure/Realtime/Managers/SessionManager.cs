using Application.Interface.Realtime.Events;
using Application.Interface.Realtime.Managers;
using Application.Interface.Utility;
using Application.Interface.Realtime.Events.Admin;
using ResponseCode;
using System.Collections.Concurrent;

namespace Infrastructure.Realtime.Managers
{
    public class SessionManager : ISessionManager
    {
        #region Attributes
        private readonly ITelemetryQueue telemetryQueue;
        private readonly IEventBus eventBus;
        private readonly ConcurrentDictionary<string, string> userToPlayer = new();
        private readonly ConcurrentDictionary<string, string> playerToUser = new();
        #endregion

        #region Properties
        #endregion

        public SessionManager(
            ITelemetryQueue telemetryQueue,
            IEventBus eventBus)
        {
            this.telemetryQueue = telemetryQueue;
            this.eventBus = eventBus;
        }

        #region Methods
        public void Add(
            string userId,
            string playerInstanceId)
        {
            if (userToPlayer.TryGetValue(userId, out var oldPlayerInstanceId))
            {
                playerToUser.TryRemove(oldPlayerInstanceId, out _);

                telemetryQueue.EnqueueAlert(
                    InfrastructureCode.SessionManagerCode.SessionOverwritten,
                    $"Session replaced for user {userId}: {oldPlayerInstanceId} -> {playerInstanceId}.",
                    TelemetrySeverity.Warning);
            }
            else
            {
                telemetryQueue.EnqueueAlert(
                    InfrastructureCode.SessionManagerCode.SessionCreated,
                    $"Session created for user {userId} with player instance {playerInstanceId}.",
                    TelemetrySeverity.Info);
            }

            userToPlayer[userId] = playerInstanceId;
            playerToUser[playerInstanceId] = userId;

            eventBus.Publish(new UserSessionChangedEvent(userId, playerInstanceId));
        }

        public string? Remove(
            string userId)
        {
            if (userToPlayer.TryRemove(userId, out var playerInstanceId))
            {
                playerToUser.TryRemove(playerInstanceId, out _);

                telemetryQueue.EnqueueAlert(
                    code: InfrastructureCode.SessionManagerCode.SessionRemoved,
                    message: $"Session removed for user {userId} and player instance {playerInstanceId}.",
                    severity: TelemetrySeverity.Info);

                eventBus.Publish(new UserSessionChangedEvent(userId, null));

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