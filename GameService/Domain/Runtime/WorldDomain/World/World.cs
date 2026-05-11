using Domain.Abstraction.World;
using Domain.Common;
using Domain.Runtime.EntityDomain;

namespace Domain.Runtime.WorldDomain.World
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

        #region Command
        public void AddEntity(
            EntityInstance entity)
        {
            // Add or update existed entity
            entities[entity.ID] = entity;

            // Register in spatial index
            spatialIndex.Add(entity);
        }

        public void RemoveEntity(
            string entityId)
        {
            if (!entities.TryGetValue(entityId, out var entity))
                return;

            // Remove from spatial first
            spatialIndex.Remove(entity);

            // Remove entity from dictionary
            entities.Remove(entityId);
        }

        public void Move(
            string entityId, 
            Vector2 newPosition, 
            int layerZ)
        {
            if (!entities.TryGetValue(entityId, out var entity))
                return;

            // Capture OLD derived state
            var oldKey = entity.GetSpatialKey();

            // Mutate authoritative state
            entity.SetPosition(newPosition, layerZ);

            // Spatial sync
            spatialIndex.Move(entity, oldKey);
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
        public IEnumerable<T> GetAll<T>() where T : EntityInstance
        {
            return entities.Values.OfType<T>();
        }

        public T? Get<T>(
            string id) where T : EntityInstance
        {
            if (!entities.TryGetValue(id, out var entity))
                return null;

            return entity as T;
        }

        public (RoomSpatial room, IEnumerable<string> entityIds) QuerySpatial(
            string roomId, 
            int x, int y, int z)
        {
            return spatialIndex.Query(roomId, x, y, z);
        }

        public RoomSpatial GetRoom(
            string roomSpatialId)
        {
            return spatialIndex.GetRoom(roomSpatialId);
        }
        #endregion
    }
}