using Domain.Runtime.WorldDomain.Spatial;
using Domain.Runtime.WorldDomain.Topology;

namespace Domain.Abstraction.World
{
    public interface IRoomCommand
    {
        void AddRoom(
            RoomSpatial roomSpatial);
        void RemoveRoom(
            string roomSpatialId);
        void AddConnection(
            RoomConnectionInstance connection);
        void RemoveConnection(
            string connectionId);
    }
}
