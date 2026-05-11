using Domain.Common;
using Domain.Document.EntityDomain;
using Domain.Runtime.EntityDomain;

namespace Application.Interfaces.Factory
{
    public interface ICreatureInstanceFactory
    {
        CreatureInstance Create(
            string definitionId,
            string instanceId,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            Vector2 direction);
        CreatureInstance CreateFromDocument(
            CreatureDocument doc);
    }
}