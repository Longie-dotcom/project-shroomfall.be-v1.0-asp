using Application.Context;
using Application.Features.Abstraction;
using Application.Features.Connection.Commands;
using Application.Interfaces.Realtime.Managers;
using Application.Persistence;
using Application.Services.WorldService;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;

namespace Application.Features.Connection.Handlers
{
    public class UnloadSessionHandler : IHandler<UnloadSessionCommand>
    {
        #region Attributes
        private readonly EntityPersistence entityPersistence;
        private readonly ResidencyService residencyService;
        private readonly PlayerContext playerContext;
        private readonly ISessionManager sessionManager;
        private readonly EntitySpawnService entitySpawnService;
        private readonly IConnectionManager connectionManager;
        private readonly WorldContext worldContext;
        #endregion

        #region Properties
        #endregion

        public UnloadSessionHandler(
            EntityPersistence entityPersistence,
            ResidencyService residencyService,
            PlayerContext playerContext,
            ISessionManager sessionManager,
            EntitySpawnService entitySpawnService,
            IConnectionManager connectionManager,
            WorldContext worldContext)
        {
            this.entityPersistence = entityPersistence;
            this.residencyService = residencyService;
            this.playerContext = playerContext;
            this.sessionManager = sessionManager;
            this.entitySpawnService = entitySpawnService;
            this.connectionManager = connectionManager;
            this.worldContext = worldContext;
        }

        #region Methods
        public async Task Handle(
            UnloadSessionCommand command)
        {
            var userId = command.UserID;
            var connectionId = command.ConnectionID;

            // Resolve live gameplay session first to know which room they are occupying
            var playerInstanceId = sessionManager.Get(userId);
            if (playerInstanceId == null)
                return; // Session was already entirely cleaned up

            // Validate player instance existence
            var player = worldContext.GetEntity(playerInstanceId);
            if (player == null)
                throw new InternalException(
                    ApplicationCode.ConnectionHandlerCode.UnloadSessionPlayerInstanceNotFound,
                    $"Player instance in runtime with instance ID: {playerInstanceId} is not found");

            // Validate transform existence
            var transform = player.GetComponent<TransformInstance>();
            if (transform == null)
                throw new InternalException(
                    ApplicationCode.ConnectionHandlerCode.UnloadSessionTransformMissing,
                    $"Player instance {playerInstanceId} is missing its TransformInstance component.");

            // 2. UNCONDITIONAL PIPE CLEANUP
            // Sever this specific connection from SignalR updates and clear it from our tracking state
            await connectionManager.Ungroup(connectionId, transform.RoomSpatialID);
            connectionManager.Remove(userId, connectionId);

            // 3. CONDITIONAL GUARD
            // Now that the dead pipe is gone, check if they are still browsing on another window or device
            if (connectionManager.HasConnections(userId))
                return; 

            // 4. GHOST CLEANUP (Executed only when ALL connections are completely gone)
            // Freeze active engine loops first
            entitySpawnService.Deactivate(player);

            // Save frozen data snapshot to cold storage second
            await entityPersistence.SaveManyAsync(new List<EntityInstance>() { player });

            // Clear tracking context variables and release the room residency lease
            FinalizeGameplaySession(player, transform, userId);
        }

        private void FinalizeGameplaySession(
            EntityInstance player,
            TransformInstance transform,
            string userId)
        {
            playerContext.LeaveRoom(transform.RoomSpatialID, player.ID);

            if (playerContext.IsRoomEmpty(transform.RoomSpatialID))
            {
                residencyService.MarkRoomExited(transform.RoomSpatialID);
            }

            sessionManager.Remove(userId);
        }
        #endregion
    }
}