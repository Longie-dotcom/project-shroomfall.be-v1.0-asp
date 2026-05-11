using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Shared;

namespace Domain.Runtime.WorldDomain.World
{
    public class SpatialIndex
    {
        #region Attributes
        private readonly Dictionary<string, RoomSpatial> rooms = new();
        #endregion

        #region Properties
        #endregion

        public SpatialIndex()
        {

        }

        #region Command
        public void Add(
            EntityInstance entity)
        {
            if (!rooms.TryGetValue(entity.RoomSpatialID, out var room))
                throw new InternalException(
                    ResponseCode.SpatialIndex_RoomSpatialNotFound,
                    $"Room spatial not registered: {entity.RoomSpatialID}");

            var key = entity.GetSpatialKey();

            room.Add(entity.ID, key.cx, key.cy, key.x, key.y, key.z);
        }

        public void Remove(
            EntityInstance entity)
        {
            if (!rooms.TryGetValue(entity.RoomSpatialID, out var room))
                return;

            var key = entity.GetSpatialKey();

            room.Remove(entity.ID, key.cx, key.cy, key.x, key.y, key.z);
        }

        public void Move(
            EntityInstance entity,
            (int cx, int cy, int x, int y, int z) oldKey)
        {
            if (!rooms.TryGetValue(entity.RoomSpatialID, out var room))
                return;

            var newKey = entity.GetSpatialKey();

            if (oldKey == newKey)
                return;

            room.Move(entity.ID, oldKey, newKey);
        }

        public void AddRoom(
            RoomSpatial roomSpatial)
        {
            if (rooms.ContainsKey(roomSpatial.ID))
                return;

            rooms[roomSpatial.ID] = roomSpatial;
        }

        public void RemoveRoom(string roomSpatialId)
        {
            rooms.Remove(roomSpatialId);
        }
        #endregion

        #region Query
        public (RoomSpatial room, IEnumerable<string> entityIds) Query(
            string roomSpatialId,
            int x, int y, int z)
        {
            if (!rooms.TryGetValue(roomSpatialId, out var room))
                throw new InternalException(
                    ResponseCode.SpatialIndex_RoomSpatialNotFound,
                    $"Room spatial not registered: {roomSpatialId}");

            return (room, room.Query(x, y, z));
        }

        public RoomSpatial GetRoom(string roomSpatialId)
        {
            if (!rooms.TryGetValue(roomSpatialId, out var room))
                throw new InternalException(
                    ResponseCode.SpatialIndex_RoomSpatialNotFound,
                    $"Room spatial not registered: {roomSpatialId}");

            return room;
        }
        #endregion
    }
}