using Application.Interfaces.Realtime;
using Application.Interfaces.Utility;
using Contract;
using Contract.DTO.Feature.Admin.Response;
using Contract.DTO.Feature.Design.Response;
using Contract.DTO.Feature.Game.Response;
using Contract.DTO.Runtime.EntityDomain;
using Contract.DTO.Runtime.EntityDomain.Component;
using Contract.DTO.Runtime.WorldDomain;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Realtime
{
    public class RealtimePublisher : IRealtimePublisher
    {
        #region Attributes
        private readonly IHubContext<GameHub> hub;
        #endregion

        #region Properties
        #endregion

        public RealtimePublisher(
            IHubContext<GameHub> hub)
        {
            this.hub = hub;
        }

        #region Methods
        // ─────────────────────────────
        // Movement (broadcast to room)
        // ─────────────────────────────
        public Task SendEntityActed(
            string roomId,
            EntityActedDTO payload)
        {
            return hub.Clients
                .Group(roomId)
                .SendAsync(NetworkMethod.OnEntityActed, payload);
        }

        // ─────────────────────────────
        // Vitals & Attributes
        // ─────────────────────────────
        public Task SendEntityVitalChanged(
            string roomId,
            EntityVitalChangedDTO payload)
        {
            return hub.Clients
                .Group(roomId)
                .SendAsync(NetworkMethod.OnEntityVitalChanged, payload);
        }

        // ─────────────────────────────
        // State Synchronization (heavy)
        // ─────────────────────────────
        public Task SendPlayerCharacteristicSync(
            string connectionId,
            CharacteristicInstanceDTO payload)
        {
            return hub.Clients
                .Client(connectionId)
                .SendAsync(NetworkMethod.OnPlayerCharacteristicSync, payload);
        }

        public Task SendInventoryItemChanged(
            string connectionId,
            InventoryItemChangedDTO payload)
        {
            return hub.Clients
                .Client(connectionId)
                .SendAsync(NetworkMethod.OnInventoryItemChanged, payload);
        }

        public Task SendInventoryCleared(
            string connectionId)
        {
            return hub.Clients
                .Client(connectionId)
                .SendAsync(NetworkMethod.OnInventoryCleared);
        }

        // ─────────────────────────────
        // Spawn & Despawn (broadcast to room)
        // ─────────────────────────────
        public Task SendEntitySpawned(
            string roomId, 
            EntityInstanceDTO entity)
        {
            return hub.Clients
                .Group(roomId)
                .SendAsync(NetworkMethod.OnEntitySpawned, entity);
        }

        public Task SendEntityDespawned(
            string roomId, 
            string entityId)
        {
            return hub.Clients
                .Group(roomId)
                .SendAsync(NetworkMethod.OnEntityDespawned, entityId);
        }

        // ─────────────────────────────
        // Player Appearance Changed (broadcast to room)
        // ─────────────────────────────
        public Task SendPlayerAppearanceChanged(
            string roomId,
            EntityAppearanceChangedDTO appearanceChanged)
        {
            return hub.Clients
                .Group(roomId)
                .SendAsync(NetworkMethod.OnPlayerAppearanceChanged, appearanceChanged);
        }

        // ─────────────────────────────
        // Room Snapshot (changed) 
        // ─────────────────────────────
        public Task SendRoomSnapshotUpdated(
            string roomId,
            RoomSpatialDTO payload)
        {
            return hub.Clients
                .Group(roomId)
                .SendAsync(NetworkMethod.OnRoomSnapshotUpdated, payload);
        }

        // ─────────────────────────────
        // Definition update (broadcast)
        // ─────────────────────────────
        public Task SendDefinitionUpdated(
            UpdateDefinitionNotificationDTO notification)
        {
            return hub.Clients
                .All
                .SendAsync(NetworkMethod.OnDefinitionUpdated, notification);
        }

        // ─────────────────────────────
        // Telemetry
        // ─────────────────────────────
        public Task SendTelemetryAlert(
            TelemetryEvent payload)
        {
            return hub.Clients
                .Group(Constraint.ADMIN_REALTIME_GROUP)
                .SendAsync(NetworkMethod.OnTelemetrySended, payload);
        }

        // ─────────────────────────────
        // Admin Dashboard Updates
        // ─────────────────────────────
        public Task SendRoomResidencyChanged(
            RoomResidencyChangedDTO payload)
        {
            return hub.Clients
                .Group(Constraint.ADMIN_REALTIME_GROUP)
                .SendAsync(NetworkMethod.OnRoomResidencyChanged, payload);
        }

        public Task SendUserConnectionChanged(
            UserConnectionChangedDTO payload)
        {
            return hub.Clients
                .Group(Constraint.ADMIN_REALTIME_GROUP)
                .SendAsync(NetworkMethod.OnUserConnectionChanged, payload);
        }

        public Task SendUserSessionChanged(
            UserSessionChangedDTO payload)
        {
            return hub.Clients
                .Group(Constraint.ADMIN_REALTIME_GROUP)
                .SendAsync(NetworkMethod.OnUserSessionChanged, payload);
        }
        #endregion
    }
}