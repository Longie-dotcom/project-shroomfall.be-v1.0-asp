using Application.Interfaces.Realtime.Events;
using Application.Interfaces.Realtime.Events.Admin;
using Application.Interfaces.Realtime.Managers;
using Application.Interfaces.Utility;
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
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> userToConnection;
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
            var connections = userToConnection.GetOrAdd(
                userId,
                _ => new ConcurrentDictionary<string, byte>());

            bool isNewUserWatchStatus = connections.IsEmpty;

            connections[connectionId] = 0;

            telemetryQueue.EnqueueAlert(
                code: InfrastructureCode.ConnectionManagerCode.ConnectionAdded,
                message: $"Connection {connectionId} added for user {userId}.",
                severity: TelemetrySeverity.Info);

            if (isNewUserWatchStatus)
                telemetryQueue.EnqueueAlert(
                    code: InfrastructureCode.ConnectionManagerCode.WatchStatusOnline,
                    message: $"User {userId} has started watching (went online).",
                    severity: TelemetrySeverity.Info);

            eventBus.Publish(new UserConnectionChangedEvent(userId, connections.Count));
        }

        public void Remove(
            string userId,
            string connectionId)
        {
            if (!userToConnection.TryGetValue(userId, out var connections))
                return;

            connections.TryRemove(connectionId, out _);

            telemetryQueue.EnqueueAlert(
                code: InfrastructureCode.ConnectionManagerCode.ConnectionRemoved,
                message: $"Connection {connectionId} removed for user {userId}.",
                severity: TelemetrySeverity.Info);

            if (connections.IsEmpty)
            {
                userToConnection.TryRemove(userId, out _);

                telemetryQueue.EnqueueAlert(
                    code: InfrastructureCode.ConnectionManagerCode.WatchStatusOffline,
                    message: $"User {userId} has stopped watching (went offline).",
                    severity: TelemetrySeverity.Info);
            }

            eventBus.Publish(new UserConnectionChangedEvent(userId, connections.Count));
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