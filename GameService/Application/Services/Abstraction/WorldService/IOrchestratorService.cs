using Domain.Common;

namespace Application.Services.Abstraction.WorldService
{
    public interface IOrchestratorService
    {
        Task SpawnNewPlayer(
            string playerDefinitionId,
            string roomDefinitionId,
            string userId);
        void SpawnPlacedWorldObject(
            string worldObjectDefinitionId,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            Vector2 direction);
        Task LoadExistedPlayer(
            string playerInstanceId);
        Task UnloadExistedPlayer(
            string playerInstanceId);
        Task EntityChangeRoom(
            string entityInstanceId,
            string targetRoomId);
    }
}
