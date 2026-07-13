using Domain.Common;
using Domain.Runtime.EntityDomain;

namespace Domain.Abstraction.World
{
    public interface IEntityCommand
    {
        void AddEntity(
            EntityInstance entityInstance);
        void RemoveEntity(
            string entityInstanceId);
        void EntityMove(
            string entityInstanceId,
            Vector2 newPosition,
            int layerZ);
        void ChangeRoom(
            string entityInstanceId,
            Vector2 newPosition,
            int layerZ,
            string newRoomSpatialId);
    }
}
