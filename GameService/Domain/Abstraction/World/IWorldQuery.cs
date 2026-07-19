using Domain.Runtime.EntityDomain;
using Domain.Runtime.WorldDomain.Spatial;

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
        IEnumerable<RoomSpatial> GetRooms();
        RoomSpatial? GetRoom(
            string roomSpatialId);
        RoomSpatial? GetRoomByOwner(
            string ownerEntityInstanceId);
    }
}
