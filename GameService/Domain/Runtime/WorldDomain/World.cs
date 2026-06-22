using Domain.Abstraction.World;
using Domain.Common;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Runtime.WorldDomain.Spatial;
using Domain.Runtime.WorldDomain.Topology;
using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;
using System.Collections;

namespace Domain.Runtime.WorldDomain
{
    public class World : IWorldQuery, IEntityCommand, IRoomCommand
    {
        #region Attributes
        private readonly Dictionary<string, EntityInstance> entities;
        private readonly Dictionary<Type, IList> entityTypeIndex;
        private readonly SpatialIndex spatialIndex;
        private readonly ConnectionTopology connectionTopology;
        #endregion

        #region Properties
        #endregion

        public World()
        {
            entities = new Dictionary<string, EntityInstance>();
            entityTypeIndex = new Dictionary<Type, IList>();
            spatialIndex = new SpatialIndex();
            connectionTopology = new ConnectionTopology();
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

        public RoomConnectionInstance? GetConnectionByEntityInstanceID(
            string entityInstanceId)
        {
            return connectionTopology.GetConnectionByEntityInstanceID(entityInstanceId);
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

            // Indexing entity
            IndexEntity(entityInstance);

            // Register in spatial index
            spatialIndex.AddEntity(transform);
        }

        public EntityInstance RemoveEntity(
            string entityInstanceId)
        {
            if (!entities.TryGetValue(entityInstanceId, out var entityInstance))
                throw new InternalException(
                    DomainCode.WorldCode.EntityInstanceNotFoundOnRemoved,
                    $"Entity instance with instance ID: {entityInstanceId} not found when on removed");

            var transform = entityInstance.GetComponent<TransformInstance>();
            if (transform == null) return entityInstance;

            // Remove from spatial first
            spatialIndex.RemoveEntity(transform);

            // Deinexing entity
            DeindexEntity(entityInstance);

            // Remove entity from dictionary
            entities.Remove(entityInstanceId);

            return entityInstance;
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

        public void AddConnection(
            RoomConnectionInstance connection)
        {
            connectionTopology.AddConnection(connection);
        }

        public void RemoveConnection(
            string connectionId)
        {
            connectionTopology.RemoveConnection(connectionId);
        }
        #endregion

        #region Helpers
        private void IndexEntity(
            EntityInstance entityInstance)
        {
            var type = entityInstance.GetType();

            while (type != null && typeof(EntityInstance).IsAssignableFrom(type))
            {
                if (!entityTypeIndex.TryGetValue(type, out var list))
                {
                    list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type))!;

                    entityTypeIndex[type] = list;
                }

                list.Add(entityInstance);

                type = type.BaseType;
            }
        }

        private void DeindexEntity(
            EntityInstance entityInstance)
        {
            var type = entityInstance.GetType();

            while (type != null && typeof(EntityInstance).IsAssignableFrom(type))
            {
                if (entityTypeIndex.TryGetValue(type, out var list))
                {
                    list.Remove(entityInstance);
                }

                type = type.BaseType;
            }
        }
        #endregion
    }
}