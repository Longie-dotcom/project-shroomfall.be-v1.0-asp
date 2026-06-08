using Application.Interfaces.Realtime;
using Contract;
using Contract.DTO.Game;
using Contract.DTO.Runtime;
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
        // Spawn (broadcast to room)
        // ─────────────────────────────
        public Task SendEntitySpawned(
            string roomId, 
            EntityRuntimeDTO entity)
        {
            return hub.Clients
                .Group(roomId)
                .SendAsync(NetworkMethod.OnEntitySpawned, entity);
        }

        // ─────────────────────────────
        // Despawn (broadcast to room)
        // ─────────────────────────────
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
            PlayerAppearanceChangedDTO appearanceChanged)
        {
            return hub.Clients
                .Group(roomId)
                .SendAsync(NetworkMethod.OnPlayerAppearanceChanged, appearanceChanged);
        }

        // ─────────────────────────────
        // Definition update (broadcast)
        // ─────────────────────────────
        public Task SendDefinitionUpdated(
            string key,
            long version)
        {
            return hub.Clients
                .All
                .SendAsync(NetworkMethod.OnDefinitionUpdated, new
                {
                    Key = key,
                    Version = version
                });
        }
        #endregion
    }
}