using Domain.Runtime.WorldDomain.Spatial;

namespace Domain.Abstraction.World
{
    public interface IRoomCommand
    {
        void AddRoom(
            RoomSpatial roomSpatial);
        void RemoveRoom(
            string roomSpatialId);
    }
}
