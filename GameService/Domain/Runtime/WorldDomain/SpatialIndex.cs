using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Shared;

namespace Domain.Runtime.WorldDomain
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
        public void AddEntity(
            EntityInstance entityInstance)
        {
            if (!rooms.TryGetValue(entityInstance.RoomSpatialID, out var room))
                throw new InternalException(
                    ResponseCode.SpatialIndex_RoomSpatialNotFoundOnEntityAdded,
                    $"Room spatial not registered: {entityInstance.RoomSpatialID}, when entity: {entityInstance.ID} was added");

            var key = entityInstance.GetSpatialKey();

            room.AddEntity(entityInstance.ID, key.cx, key.cy, key.x, key.y, key.z);
        }

        public void RemoveEntity(
            EntityInstance entityInstance)
        {
            if (!rooms.TryGetValue(entityInstance.RoomSpatialID, out var room))
                throw new InternalException(
                    ResponseCode.SpatialIndex_RoomSpatialNotFoundOnEntityRemoved,
                    $"Room spatial not registered: {entityInstance.RoomSpatialID}, when entity: {entityInstance.ID} was removed");

            var key = entityInstance.GetSpatialKey();

            room.RemoveEntity(entityInstance.ID, key.cx, key.cy, key.x, key.y, key.z);
        }

        public void EntityMove(
            EntityInstance entityInstance,
            (int cx, int cy, int x, int y, int z) oldKey)
        {
            if (!rooms.TryGetValue(entityInstance.RoomSpatialID, out var room))
                throw new InternalException(
                    ResponseCode.SpatialIndex_RoomSpatialNotFoundOnEntityMoved,
                    $"Room spatial not registered: {entityInstance.RoomSpatialID}, when entity: {entityInstance.ID} moved");

            var newKey = entityInstance.GetSpatialKey();
            if (oldKey == newKey)
                return;

            room.EntityMove(entityInstance.ID, oldKey, newKey);
        }

        public void AddRoom(
            RoomSpatial roomSpatial)
        {
            rooms[roomSpatial.ID] = roomSpatial;
        }

        public void RemoveRoom(string roomSpatialId)
        {
            rooms.Remove(roomSpatialId);
        }
        #endregion

        #region Query
        public (RoomSpatial?, IEnumerable<string>) Query(
            string roomSpatialId,
            int x, int y, int z)
        {
            return rooms.TryGetValue(roomSpatialId, out var roomSpatial)
                ? (roomSpatial, roomSpatial.Query(x, y, z))
                : (null, new List<string>());
        }

        public RoomSpatial? GetRoom(string roomSpatialId)
        {
            return rooms.TryGetValue(roomSpatialId, out var roomSpatial)
                ? roomSpatial
                : null;
        }
        #endregion
    }
}