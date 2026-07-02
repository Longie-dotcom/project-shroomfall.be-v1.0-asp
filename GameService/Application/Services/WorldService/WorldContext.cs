using Application.Interfaces.Utility;
using Domain.Abstraction.World;
using Domain.Common;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Runtime.WorldDomain.Spatial;
using Domain.Shared.ResponseCode;

namespace Application.Services.WorldService
{
    public class WorldContext
    {
        #region Attributes
        private readonly ITelemetryQueue telemetryQueue;
        private readonly IWorldQuery worldQuery;
        private readonly IEntityCommand entityCommand;
        private readonly IRoomCommand roomCommand;
        #endregion

        #region Properties
        #endregion

        public WorldContext(
            ITelemetryQueue telemetryQueue,
            IWorldQuery worldQuery,
            IEntityCommand entityCommand,
            IRoomCommand roomCommand)
        {
            this.telemetryQueue = telemetryQueue;
            this.worldQuery = worldQuery;
            this.entityCommand = entityCommand;
            this.roomCommand = roomCommand;
        }

        #region Query
        public IEnumerable<EntityInstance> GetEntities()
        {
            return worldQuery.GetEntities();
        }

        public EntityInstance? GetEntity(
            string entityInstanceId)
        {
            return worldQuery.GetEntity(entityInstanceId);
        }

        public RoomSpatial? GetRoom(
            string roomSpatialId)
        {
            return worldQuery.GetRoom(roomSpatialId);
        }

        public (RoomSpatial?, IEnumerable<EntityInstance>) QuerySpatial(
            string roomSpatialId,
            int x, int y, int z)
        {
            return worldQuery.QuerySpatial(roomSpatialId, x, y, z);
        }

        public List<EntityInstance> GetEntitiesByRoom(
            string roomSpatialId)
        {
            return worldQuery
                .GetEntities()
                .Where(e => e.GetComponent<TransformInstance>()?.RoomSpatialID == roomSpatialId)
                .ToList();
        }

        public RoomSpatial? GetRoomByOwner(
            string ownerEntityInstanceId)
        {
            return worldQuery.GetRoomByOwner(ownerEntityInstanceId);
        }
        #endregion

        #region Command
        public void Load(
            RoomSnapshot roomSnapshot)
        {
            // Load room first
            roomCommand.AddRoom(roomSnapshot.Room);

            // Then load entities
            foreach (var entityInstance in roomSnapshot.Entities)
            {
                AddEntity(entityInstance);
            }

            telemetryQueue.EnqueueAlert(
                ApplicationCode.WorldContextCode.RoomLoaded,
                $"Successfully loading room '{roomSnapshot.Room.ID}' into World...",
                TelemetrySeverity.Info);
        }

        public RoomSnapshot? Unload(
            string roomSpatialId)
        {
            // Fetch room need to be unloaded
            var room = worldQuery.GetRoom(roomSpatialId);
            if (room == null)
            {
                telemetryQueue.EnqueueAlert(
                    ApplicationCode.WorldContextCode.UnloadTargetMissing,
                    $"Attempted to unload room '{roomSpatialId}', but it was not found...",
                    TelemetrySeverity.Warning);

                return null;
            }

            // Fetch related entities inside that unloaded room
            var entities = GetEntitiesByRoom(roomSpatialId);

            // Remove entities first
            foreach (var entityInstance in entities)
            {
                RemoveEntity(entityInstance.ID);
            }

            // Remove room
            roomCommand.RemoveRoom(roomSpatialId);

            telemetryQueue.EnqueueAlert(
                ApplicationCode.WorldContextCode.RoomUnloading,
                $"Evicting room '{roomSpatialId}' from World...",
                TelemetrySeverity.Info);

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

            telemetryQueue.EnqueueAlert(
                ApplicationCode.WorldContextCode.EntityRoomChanged,
                $"Entity '{entityInstanceId}' changed room to room spatial: {newRoomSpatialId} on new position: ({newPosition.X}, {newPosition.Y}, {layerZ}",
                TelemetrySeverity.Info);
        }
        #endregion
    }
}