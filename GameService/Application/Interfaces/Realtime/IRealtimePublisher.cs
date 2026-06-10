using Contract.DTO.Game;
using Contract.DTO.Runtime;

namespace Application.Interfaces.Realtime
{
    public interface IRealtimePublisher
    {
        // ─────────────────────────────
        // Acted (high frequency)
        // ─────────────────────────────
        Task SendEntityActed(
            string roomSpatialId, 
            EntityActedDTO payload);

        // ─────────────────────────────
        // Vitals & Attributes
        // ─────────────────────────────
        Task SendEntityVitalChanged(
            string roomSpatialId,
            EntityVitalChangedDTO payload);

        // ─────────────────────────────
        // State Synchronization (heavy)
        // ─────────────────────────────
        Task SendPlayerCharacteristicSync(
            string connectionId,
            CharacteristicRuntimeDTO payload);

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
        // Player Appearance (changed) 
        // ─────────────────────────────
        Task SendPlayerAppearanceChanged(
            string roomSpatialId,
            EntityAppearanceChangedDTO appearanceChanged);

        // ─────────────────────────────
        // Definition Update Notification
        // ─────────────────────────────
        Task SendDefinitionUpdated(
            string key,
            long version);
    }
}