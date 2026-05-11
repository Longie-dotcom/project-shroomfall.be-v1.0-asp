using Application.DTO.Game;
using Application.DTO.Runtime;

namespace Application.Interfaces.Realtime
{
    public interface IRealtimePublisher
    {
        // ─────────────────────────────
        // Movement (high frequency)
        // ─────────────────────────────
        Task SendEntityMoved(string roomId, EntityMovedDTO payload);

        // ─────────────────────────────
        // Lifecycle (spawn / despawn)
        // ─────────────────────────────
        Task SendEntitySpawned(string roomId, EntityRuntimeDTO entity);
        Task SendEntityDespawned(string roomId, string entityId);

        Task SendDefinitionUpdated(
            string key,
            long version);

        // ─────────────────────────────
        // (Optional future) delta sync
        // ─────────────────────────────
        Task SendDelta(string connectionId, object delta);
    }
}