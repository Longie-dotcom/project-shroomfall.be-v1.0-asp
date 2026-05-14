using Domain.Runtime.EntityDomain;
using Domain.Runtime.WorldDomain;

namespace Domain.Abstraction.World
{
    public interface IWorldQuery
    {
        IEnumerable<T> GetEntities<T>() where T : EntityInstance;
        T? GetEntity<T>(
            string entityInstanceId) where T : EntityInstance;
        (RoomSpatial?, IEnumerable<string>) QuerySpatial(
            string roomSpatialId, 
            int x, int y, int z);
        RoomSpatial? GetRoom(
            string roomSpatialId);
    }
}
