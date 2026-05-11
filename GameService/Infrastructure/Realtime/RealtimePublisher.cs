using Application.DTO.Game;
using Application.DTO.Runtime;
using Application.Interfaces.Realtime;
using Microsoft.AspNetCore.SignalR;
using SignalHub;

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
        public Task SendEntityMoved(string roomId, EntityMovedDTO payload)
        {
            return hub.Clients
                .Group(roomId)
                .SendAsync("EntityMoved", payload);
        }

        // ─────────────────────────────
        // Spawn (broadcast to room)
        // ─────────────────────────────
        public Task SendEntitySpawned(string roomId, EntityRuntimeDTO entity)
        {
            return hub.Clients
                .Group(roomId)
                .SendAsync("EntitySpawned", entity);
        }

        // ─────────────────────────────
        // Despawn (broadcast to room)
        // ─────────────────────────────
        public Task SendEntityDespawned(string roomId, string entityId)
        {
            return hub.Clients
                .Group(roomId)
                .SendAsync("EntityDespawned", entityId);
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
                .SendAsync("DefinitionUpdated", new
                {
                    Key = key,
                    Version = version
                });
        }

        // ─────────────────────────────
        // Delta (ONLY caller)
        // ─────────────────────────────
        public Task SendDelta(string connectionId, object delta)
        {
            return hub.Clients
                .Client(connectionId)
                .SendAsync("Delta", delta);
        }
        #endregion
    }
}