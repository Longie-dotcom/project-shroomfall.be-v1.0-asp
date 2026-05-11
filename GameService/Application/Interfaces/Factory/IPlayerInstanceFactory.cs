using Domain.Common;
using Domain.Document.EntityDomain;
using Domain.Runtime.EntityDomain;

namespace Application.Interfaces.Factory
{
    public interface IPlayerInstanceFactory
    {
        PlayerInstance Create(
            string definitionId,
            string instanceId,
            string userId,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            Vector2 direction);
        PlayerInstance CreateFromDocument(
            PlayerDocument doc);
    }
}
