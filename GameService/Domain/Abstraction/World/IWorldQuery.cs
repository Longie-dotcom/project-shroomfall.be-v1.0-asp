using Domain.Runtime.EntityDomain;
using Domain.Runtime.WorldDomain.Spatial;
using Domain.Runtime.WorldDomain.Topology;

namespace Domain.Abstraction.World
{
    public interface IWorldQuery
    {
        IEnumerable<EntityInstance> GetEntities();
        EntityInstance? GetEntity(
            string entityInstanceId);
        (RoomSpatial?, IEnumerable<EntityInstance>) QuerySpatial(
            string roomSpatialId, 
            int x, int y, int z);
        RoomSpatial? GetRoom(
            string roomSpatialId);
        RoomConnectionInstance? GetConnectionByEntityInstanceID(
            string entityInstanceId);
    }
}
