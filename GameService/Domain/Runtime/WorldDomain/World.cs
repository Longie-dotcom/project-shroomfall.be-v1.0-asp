using Domain.Abstraction.World;
using Domain.Common;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Shared;
using System.Collections;

namespace Domain.Runtime.WorldDomain
{
    public class World : IWorldQuery, IEntityCommand, IRoomCommand
    {
        #region Attributes
        private readonly Dictionary<string, EntityInstance> entities;
        private readonly Dictionary<Type, IList> entityTypeIndex;
        private readonly SpatialIndex spatialIndex;
        #endregion

        #region Properties
        #endregion

        public World()
        {
            entities = new Dictionary<string, EntityInstance>();
            entityTypeIndex = new Dictionary<Type, IList>();
            spatialIndex = new SpatialIndex();
        }

        #region Command
        public void AddEntity(
            EntityInstance entityInstance)
        {
            // Add or update existed entity
            entities[entityInstance.ID] = entityInstance;

            // Indexing entity
            IndexEntity(entityInstance);

            // Register in spatial index
            spatialIndex.AddEntity(entityInstance);
        }

        public EntityInstance RemoveEntity(
            string entityInstanceId)
        {
            if (!entities.TryGetValue(entityInstanceId, out var entityInstance))
                throw new InternalException(
                    ResponseCode.World_EntityInstanceNotFoundOnRemoved,
                    $"Entity instance with instance ID: {entityInstanceId} not found when on removed");

            // Remove from spatial first
            spatialIndex.RemoveEntity(entityInstance);

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
                    ResponseCode.World_EntityInstanceNotFoundOnMoved,
                    $"Entity instance with instance ID: {entityInstanceId} not found when move");

            // Capture OLD derived state
            var oldKey = entityInstance.GetSpatialKey();

            // Mutate authoritative state
            entityInstance.SetPosition(newPosition, layerZ);

            // Spatial sync
            spatialIndex.EntityMove(entityInstance, oldKey);
        }

        public void ChangeRoom(
            string entityInstanceId,
            Vector2 newPosition,
            int layerZ,
            string newRoomSpatialId)
        {
            if (!entities.TryGetValue(entityInstanceId, out var entityInstance))
                throw new InternalException(
                    ResponseCode.World_EntityInstanceNotFoundOnRoomChanged,
                    $"Entity instance with instance ID: {entityInstanceId} not found when changed room");

            // Remove from OLD room spatial index
            spatialIndex.RemoveEntity(entityInstance);

            // Mutate authoritative state
            entityInstance.ChangeRoom(
                newRoomSpatialId,
                newPosition,
                layerZ);

            // Register into NEW room spatial index
            spatialIndex.AddEntity(entityInstance);
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

        #region Query
        public IEnumerable<T> GetEntities<T>() where T : EntityInstance
        {
            if (!entityTypeIndex.TryGetValue(typeof(T), out var list))
                return Enumerable.Empty<T>();

            return ((List<T>)list);
        }

        public T? GetEntity<T>(
            string entityInstanceId) where T : EntityInstance
        {
            if (!entities.TryGetValue(entityInstanceId, out var entity))
                return null;

            return entity as T;
        }

        public (RoomSpatial?, IEnumerable<string>) QuerySpatial(
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