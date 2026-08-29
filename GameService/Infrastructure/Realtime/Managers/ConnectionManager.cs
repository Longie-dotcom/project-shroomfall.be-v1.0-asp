using Application.Interface.Realtime.Events;
using Application.Interface.Realtime.Managers;
using Application.Interface.Utility;
using Application.Interface.Realtime.Events.Admin;
using Domain.DomainException;
using Microsoft.AspNetCore.SignalR;
using ResponseCode;
using System.Collections.Concurrent;

namespace Infrastructure.Realtime.Managers
{
    public class ConnectionManager : IConnectionManager
    {
        #region Attributes
        private readonly ITelemetryQueue telemetryQueue;
        private readonly IEventBus eventBus;
        private readonly IHubContext<GameHub> hub;
        private readonly ConcurrentDictionary<string, string> userToConnection;
        #endregion

        #region Properties
        #endregion

        public ConnectionManager(
            ITelemetryQueue telemetryQueue,
            IEventBus eventBus,
            IHubContext<GameHub> hub)
        {
            this.telemetryQueue = telemetryQueue;
            this.eventBus = eventBus;
            this.hub = hub;
            userToConnection = new();
        }

        #region Methods
        public void Add(
            string userId,
            string connectionId)
        {
            if (userToConnection.TryGetValue(userId, out var oldConnectionId))
            {
                telemetryQueue.EnqueueAlert(
                    InfrastructureCode.ConnectionManagerCode.ConnectionReplaced,
                    $"Connection replaced for user {userId}: {oldConnectionId} to {connectionId}.",
                    TelemetrySeverity.Warning);
            }
            else
            {
                telemetryQueue.EnqueueAlert(
                    InfrastructureCode.ConnectionManagerCode.ConnectionAdded,
                    $"Connection {connectionId} added for user {userId}.",
                    TelemetrySeverity.Info);
            }

            userToConnection[userId] = connectionId;

            eventBus.Publish(new UserConnectionChangedEvent(userId, connectionId));
        }

        public void Remove(
            string userId,
            string connectionId)
        {
            if (!userToConnection.TryGetValue(userId, out var currentConnectionId))
                throw new BadRequest(
                    InfrastructureCode.ConnectionManagerCode.ConnectionNotFound,
                    $"User {userId} does not have an active connection.");

            if (currentConnectionId != connectionId)
                throw new BadRequest(
                    InfrastructureCode.ConnectionManagerCode.ConnectionMismatch,
                    $"Connection {connectionId} does not match the active connection for user {userId}.");

            userToConnection.TryRemove(userId, out _);

            telemetryQueue.EnqueueAlert(
                code: InfrastructureCode.ConnectionManagerCode.ConnectionRemoved,
                message: $"Connection {connectionId} removed for user {userId}.",
                severity: TelemetrySeverity.Info);

            eventBus.Publish(new UserConnectionChangedEvent(userId, null));
        }

        public string? Get(
            string userId)
        {
            return userToConnection.TryGetValue(userId, out var connectionId)
                ? connectionId
                : null;
        }

        public Task Group(
            string connectionId,
            string groupId)
        {
            return hub.Groups.AddToGroupAsync(connectionId, groupId);
        }

        public Task Ungroup(
            string connectionId,
            string groupId)
        {
            return hub.Groups.RemoveFromGroupAsync(connectionId, groupId);
        }
        #endregion
    }
}