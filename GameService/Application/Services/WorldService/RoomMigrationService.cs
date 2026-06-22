using Application.Context;
using Application.Interfaces.Realtime.Managers;
using Domain.Common;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;

namespace Application.Services.WorldService
{
    public class RoomMigrationService
    {
        #region Attributes
        private readonly ResidencyService residencyService;
        private readonly PlayerContext playerContext;
        private readonly EntitySpawnService entitySpawnService;
        private readonly IConnectionManager connectionManager;
        #endregion

        public RoomMigrationService(
            ResidencyService residencyService,
            PlayerContext playerContext,
            EntitySpawnService entitySpawnService,
            IConnectionManager connectionManager)
        {
            this.residencyService = residencyService;
            this.playerContext = playerContext;
            this.entitySpawnService = entitySpawnService;
            this.connectionManager = connectionManager;
        }

        #region Methods
        public async Task ExecuteMigrationAsync(
            EntityInstance player,
            TransformInstance transform,
            string toRoomSpatialId,
            Vector2 spawnPosition,
            int layerZ)
        {
            var fromRoomId = transform.RoomSpatialID;

            // Bring target space into RAM
            await residencyService.EnsureRoomLoaded(toRoomSpatialId);

            // Physical server-side spatial mutation
            entitySpawnService.TransitionRoom(player, toRoomSpatialId, spawnPosition, layerZ);

            // Keep application contexts in sync
            playerContext.LeaveRoom(fromRoomId, player.ID);
            playerContext.JoinRoom(toRoomSpatialId, player.ID);

            // Update memory lifecycles
            residencyService.MarkRoomHot(toRoomSpatialId);
            if (playerContext.IsRoomEmpty(fromRoomId))
                residencyService.MarkRoomExited(fromRoomId);
            else
                residencyService.TouchRoom(fromRoomId);

            // Network Socket Group Swap
            var ownership = player.GetComponent<OwnershipInstance>();
            if (ownership != null)
            {
                var activeConnections = connectionManager.Get(ownership.UserID);
                foreach (var connectionId in activeConnections)
                {
                    await connectionManager.Ungroup(connectionId, fromRoomId);
                    await connectionManager.Group(connectionId, toRoomSpatialId);
                }
            }
        }
        #endregion
    }
}