using Domain.Abstraction.World;
using Domain.Common;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Runtime.WorldDomain.Spatial;
using ResponseCode;

namespace Domain.Runtime.WorldDomain
{
    public class World : IWorldQuery, IEntityCommand, IRoomCommand
    {
        #region Attributes
        private readonly Dictionary<string, EntityInstance> entities;
        private readonly SpatialIndex spatialIndex;
        #endregion

        #region Properties
        #endregion

        public World()
        {
            entities = new Dictionary<string, EntityInstance>();
            spatialIndex = new SpatialIndex();
        }

        #region Query
        public IEnumerable<EntityInstance> GetEntities()
        {
            return entities.Values;
        }

        public EntityInstance? GetEntity(
            string entityInstanceId)
        {
            if (!entities.TryGetValue(entityInstanceId, out var entity))
                return null;

            return entity;
        }

        public (RoomSpatial?, IEnumerable<EntityInstance>) QuerySpatial(
            string roomSpatialId,
            int x, int y, int z)
        {
            return spatialIndex.Query(roomSpatialId, x, y, z);
        }

        public RoomSpatial? GetRoom(
            string roomSpatialId)
        {
            return spatialIndex.GetRoom(roomSpatialId);
        }

        public RoomSpatial? GetRoomByOwner(
            string ownerEntityInstanceId)
        {
            return spatialIndex.GetRoomByOwner(ownerEntityInstanceId);
        }
        #endregion

        #region Command
        public void AddEntity(
            EntityInstance entityInstance)
        {
            var transform = entityInstance.GetComponent<TransformInstance>();
            if (transform == null) return;

            // Add or update existed entity
            entities[entityInstance.ID] = entityInstance;

            // Register in spatial index
            spatialIndex.AddEntity(transform);
        }

        public void RemoveEntity(
            string entityInstanceId)
        {
            if (!entities.TryGetValue(entityInstanceId, out var entityInstance))
                return;

            var transform = entityInstance.GetComponent<TransformInstance>();
            if (transform == null) return;

            // Remove from spatial first
            spatialIndex.RemoveEntity(transform);

            // Remove entity from dictionary
            entities.Remove(entityInstanceId);

            return;
        }

        public void EntityMove(
            string entityInstanceId, 
            Vector2 newPosition, 
            int layerZ)
        {
            if (!entities.TryGetValue(entityInstanceId, out var entityInstance))
                throw new InternalException(
                    DomainCode.WorldCode.EntityInstanceNotFoundOnMoved,
                    $"Entity instance with instance ID: {entityInstanceId} not found when move");

            var transform = entityInstance.GetComponent<TransformInstance>();
            if (transform == null) return;

            // Capture OLD derived state
            var oldKey = transform.GetSpatialKey();

            // Mutate authoritative state
            transform.SetPosition(newPosition, layerZ);

            // Spatial sync
            spatialIndex.EntityMove(transform, oldKey);
        }

        public void ChangeRoom(
            string entityInstanceId,
            Vector2 newPosition,
            int layerZ,
            string newRoomSpatialId)
        {
            if (!entities.TryGetValue(entityInstanceId, out var entityInstance))
                throw new InternalException(
                    DomainCode.WorldCode.EntityInstanceNotFoundOnRoomChanged,
                    $"Entity instance with instance ID: {entityInstanceId} not found when changed room");

            var transform = entityInstance.GetComponent<TransformInstance>();
            if (transform == null) return;

            // Remove from OLD room spatial index
            spatialIndex.RemoveEntity(transform);

            // Mutate authoritative state
            transform.ChangeRoom(
                newRoomSpatialId,
                newPosition,
                layerZ);

            // Register into NEW room spatial index
            spatialIndex.AddEntity(transform);
        }

        public void AddRoom(
            RoomSpatial roomSpatial)
        {
            spatialIndex.AddRoom(roomSpatial);
        }

        public void RemoveRoom(
            string roomSpatialId)
        {
            spatialIndex.RemoveRoom(roomSpatialId);
        }
        #endregion
    }
}