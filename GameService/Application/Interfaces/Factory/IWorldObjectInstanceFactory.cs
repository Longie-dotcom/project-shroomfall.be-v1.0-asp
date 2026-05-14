using Domain.Common;
using Domain.Document.EntityDomain;
using Domain.Runtime.EntityDomain;

namespace Application.Interfaces.Factory
{
    public interface IWorldObjectInstanceFactory
    {
        (WorldObjectInstance worldObject, string? linkedRoomSpatialId, string? linkedRoomDefinitionId) Create(
            string definitionId,
            string instanceId,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            Vector2 direction);
        WorldObjectInstance CreateFromDocument(
            WorldObjectDocument doc);
    }
}
