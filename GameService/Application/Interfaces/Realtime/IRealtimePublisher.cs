using Application.DTO.Game;
using Application.DTO.Runtime;

namespace Application.Interfaces.Realtime
{
    public interface IRealtimePublisher
    {
        // ─────────────────────────────
        // Movement (high frequency)
        // ─────────────────────────────
        Task SendEntityMoved(
            string roomSpatialId, 
            EntityMovedDTO payload);

        // ─────────────────────────────
        // Lifecycle (spawn / despawn)
        // ─────────────────────────────
        Task SendEntitySpawned(
            string roomSpatialId, 
            EntityRuntimeDTO entity);
        Task SendEntityDespawned(
            string roomSpatialId, 
            string entityId);

        // ─────────────────────────────
        // Definition Update Notification
        // ─────────────────────────────
        Task SendDefinitionUpdated(
            string key,
            long version);
    }
}