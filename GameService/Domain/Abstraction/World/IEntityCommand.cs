using Domain.Common;
using Domain.Runtime.EntityDomain;

namespace Domain.Abstraction.World
{
    public interface IEntityCommand
    {
        void AddEntity(
            EntityInstance entity);
        void RemoveEntity(
            string entityId);
        void Move(
            string entityId,
            Vector2 newPosition,
            int layerZ);
    }
}
