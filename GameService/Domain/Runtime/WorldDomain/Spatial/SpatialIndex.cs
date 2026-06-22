using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;

namespace Domain.Runtime.WorldDomain.Spatial
{
    public class SpatialIndex
    {
        #region Attributes
        private readonly Dictionary<string, RoomSpatial> rooms = new();
        #endregion

        #region Properties
        #endregion

        public SpatialIndex() { }

        #region Command
        public void AddEntity(
            TransformInstance transform)
        {
            if (!rooms.TryGetValue(transform.RoomSpatialID, out var room))
                throw new InternalException(
                    DomainCode.SpatialIndexCode.RoomSpatialNotFoundOnEntityAdded,
                    $"Room spatial not registered: {transform.RoomSpatialID}, when entity: {transform.Entity.ID} was added");

            var key = transform.GetSpatialKey();

            room.AddEntity(transform.Entity, key.cx, key.cy, key.x, key.y, key.z);
        }

        public void RemoveEntity(
            TransformInstance transform)
        {
            if (!rooms.TryGetValue(transform.RoomSpatialID, out var room))
                throw new InternalException(
                    DomainCode.SpatialIndexCode.RoomSpatialNotFoundOnEntityRemoved,
                    $"Room spatial not registered: {transform.RoomSpatialID}, when entity: {transform.Entity.ID} was removed");

            var key = transform.GetSpatialKey();

            room.RemoveEntity(transform.Entity, key.cx, key.cy, key.x, key.y, key.z);
        }

        public void EntityMove(
            TransformInstance transform,
            (int cx, int cy, int x, int y, int z) oldKey)
        {
            if (!rooms.TryGetValue(transform.RoomSpatialID, out var room))
                throw new InternalException(
                    DomainCode.SpatialIndexCode.RoomSpatialNotFoundOnEntityMoved,
                    $"Room spatial not registered: {transform.RoomSpatialID}, when entity: {transform.Entity.ID} moved");
            
            var newKey = transform.GetSpatialKey();
            if (oldKey == newKey)
                return;

            room.EntityMove(transform.Entity, oldKey, newKey);
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
        public (RoomSpatial?, IEnumerable<EntityInstance>) Query(
            string roomSpatialId,
            int x, int y, int z)
        {
            return rooms.TryGetValue(roomSpatialId, out var roomSpatial)
                ? (roomSpatial, roomSpatial.Query(x, y, z))
                : (null, new List<EntityInstance>());
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