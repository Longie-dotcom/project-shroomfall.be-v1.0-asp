using Domain.Common;
using Domain.Runtime.EntityDomain;

namespace Application.Interfaces.Factory
{
    public interface IAreaEffectInstanceFactory
    {
        AreaEffectInstance Create(
            string definitionId,
            string instanceId,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            Vector2 movementVector,
            string entityInstanceOwnerId,
            string? sourceDefinitionId);
    }
}