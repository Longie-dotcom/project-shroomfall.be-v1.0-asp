using Application.Interfaces.Realtime.Events;
using Application.Interfaces.Realtime.Events.Admin;
using Application.Interfaces.Realtime.Managers;
using Application.Interfaces.Utility;
using Domain.Shared.ResponseCode;
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
            if (userToPlayer.TryRemove(userId, out var oldPlayerId))
            {
                playerToUser.TryRemove(oldPlayerId, out _);

                telemetryQueue.EnqueueAlert(
                    code: InfrastructureCode.SessionManagerCode.SessionOverwritten,
                    message: $"User {userId} overwrote previous player instance {oldPlayerId} with {playerInstanceId}.",
                    severity: TelemetrySeverity.Debug);
            }

            userToPlayer[userId] = playerInstanceId;
            playerToUser[playerInstanceId] = userId;

            telemetryQueue.EnqueueAlert(
                code: InfrastructureCode.SessionManagerCode.SessionCreated,
                message: $"User {userId} mapped to player instance {playerInstanceId}.",
                severity: TelemetrySeverity.Info);

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