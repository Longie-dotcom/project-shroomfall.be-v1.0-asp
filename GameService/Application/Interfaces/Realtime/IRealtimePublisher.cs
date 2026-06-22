using Application.Interfaces.Utility;
using Contract.DTO.Admin;
using Contract.DTO.Design;
using Contract.DTO.Domain.Runtime;
using Contract.DTO.Game;

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
            CharacteristicInstanceDTO payload);

        // ─────────────────────────────
        // Lifecycle (spawn / despawn)
        // ─────────────────────────────
        Task SendEntitySpawned(
            string roomSpatialId, 
            EntityInstanceDTO entity);
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
            UpdateDefinitionNotificationDTO notification);

        // ─────────────────────────────
        // Telemetry
        // ─────────────────────────────
        Task SendTelemetryAlert(
            TelemetryEvent payload);

        // ─────────────────────────────
        // Admin Dashboard Updates
        // ─────────────────────────────
        Task SendRoomResidencyChanged(
            RoomResidencyChangedDTO payload);
        Task SendUserConnectionChanged(
            UserConnectionChangedDTO payload);
        Task SendUserSessionChanged(
            UserSessionChangedDTO payload);
    }
}