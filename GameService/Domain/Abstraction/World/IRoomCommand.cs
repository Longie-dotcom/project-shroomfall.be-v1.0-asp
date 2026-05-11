using Domain.Runtime.WorldDomain.World;

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
