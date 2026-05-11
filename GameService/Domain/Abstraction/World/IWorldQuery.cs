using Domain.Runtime.EntityDomain;
using Domain.Runtime.WorldDomain.World;
namespace Domain.Abstraction.World
{
    public interface IWorldQuery
    {
        IEnumerable<T> GetAll<T>() where T : EntityInstance;
        T? Get<T>(
            string id) where T : EntityInstance;
        (RoomSpatial room, IEnumerable<string> entityIds) QuerySpatial(
            string roomSpatialId, 
            int x, int y, int z);
        RoomSpatial GetRoom(
            string roomSpatialId);
    }
}
