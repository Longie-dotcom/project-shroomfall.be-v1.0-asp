using Domain.Runtime.WorldDomain;

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
