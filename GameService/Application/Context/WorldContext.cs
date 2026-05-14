using Application.Persistence;
using Application.Services.WorldService;
using Domain.Abstraction.World;
using Domain.Common;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.WorldDomain;

namespace Application.Context
{
    public class WorldContext
    {
        #region Attributes
        private readonly IWorldQuery worldQuery;
        private readonly IEntityCommand entityCommand;
        private readonly IRoomCommand roomCommand;
        #endregion

        #region Properties
        #endregion

        public WorldContext(
            IWorldQuery worldQuery,
            IEntityCommand entityCommand,
            IRoomCommand roomCommand)
        {
            this.worldQuery = worldQuery;
            this.entityCommand = entityCommand;
            this.roomCommand = roomCommand;
        }

        #region Methods
        public void Load(
            WorldGraph graph)
        {
            // Load rooms first
            foreach (var room in graph.Rooms)
            {
                roomCommand.AddRoom(room);
            }

            // Then load entities
            foreach (var entityInstance in graph.Entities)
            {
                AddEntity(entityInstance);
            }
        }

        public RoomSnapshot? Unload(
            string roomSpatialId)
        {
            // Fetch room need to be unloaded
            var room = worldQuery.GetRoom(roomSpatialId);
            if (room == null)
                return null;

            // Fetch related entities inside that unloaded room
            var entities = GetEnvironmentEntitiesByRoom(roomSpatialId);

            // Remove entities first
            foreach (var entityInstance in entities)
            {
                RemoveEntity(entityInstance.ID);
            }

            // Remove room
            roomCommand.RemoveRoom(roomSpatialId);

            // Return snapshot for persist
            return new RoomSnapshot
            {
                Room = room,
                Entities = entities
            };
        }

        public void AddEntity(
            EntityInstance entityInstance)
        {
            entityCommand.AddEntity(entityInstance);
        }

        public void RemoveEntity(
            string entityInstanceId)
        {
            entityCommand.RemoveEntity(entityInstanceId);
        }

        public void EntityMove(
            string entityInstanceId,
            Vector2 newPosition,
            int layerZ)
        {
            entityCommand.EntityMove(
                entityInstanceId,
                newPosition,
                layerZ);
        }

        public void ChangeRoom(
            string entityInstanceId,
            Vector2 newPosition,
            int layerZ,
            string newRoomSpatialId)
        {
            entityCommand.ChangeRoom(
                entityInstanceId,
                newPosition,
                layerZ,
                newRoomSpatialId);
        }


        public IEnumerable<T> GetEntities<T>() where T : EntityInstance
        {
            return worldQuery.GetEntities<T>();
        }

        public T? GetEntity<T>(
            string entityInstanceId) where T : EntityInstance
        {
            return worldQuery.GetEntity<T>(entityInstanceId);
        }

        public RoomSpatial? GetRoom(
            string roomSpatialId)
        {
            return worldQuery.GetRoom(roomSpatialId);
        }

        public (RoomSpatial?, IEnumerable<string>) QuerySpatial(
            string roomSpatialId,
            int x, int y, int z)
        {
            return worldQuery.QuerySpatial(roomSpatialId, x, y, z);
        }

        public List<EntityInstance> GetEnvironmentEntitiesByRoom(
            string roomSpatialId)
        {
            return worldQuery
                .GetEntities<EntityInstance>()
                .Where(e =>
                    e.RoomSpatialID == roomSpatialId &&
                    e is not PlayerInstance)
                .ToList();
        }

        public List<EntityInstance> GetEntitiesByRoom(
            string roomSpatialId)
        {
            return worldQuery
                .GetEntities<EntityInstance>()
                .Where(e => e.RoomSpatialID == roomSpatialId)
                .ToList();
        }
        #endregion
    }
}