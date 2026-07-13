using Application.Interfaces.Realtime.Events;
using Application.Interfaces.Realtime.Events.Admin;
using Application.Interfaces.Realtime.Managers;
using Application.Interfaces.Utility;
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
            if (userToConnection.ContainsKey(userId))
                throw new BadRequest(
                    InfrastructureCode.ConnectionManagerCode.ConnectionAlreadyExists,
                    $"User {userId} already has an active connection.");

            userToConnection[userId] = connectionId;

            telemetryQueue.EnqueueAlert(
                code: InfrastructureCode.ConnectionManagerCode.ConnectionAdded,
                message: $"Connection {connectionId} added for user {userId}.",
                severity: TelemetrySeverity.Info);

            eventBus.Publish(new UserConnectionChangedEvent(userId, 1));
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

            eventBus.Publish(new UserConnectionChangedEvent(userId, 0));
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