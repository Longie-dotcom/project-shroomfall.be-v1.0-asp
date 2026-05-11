using Domain.Common;

namespace Application.Services.Abstraction.WorldService
{
    public interface ICreationService
    {
        WorldContext CreatePlayerContext(
            string playerDefinitionId,
            string roomDefinitionId,
            string userId);
        WorldContext CreatePlacedWorldObjectContext(
            string worldObjectDefinitionId,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            Vector2 direction);
    }
}
