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
            ISessionManager sessionManager,
            EntitySpawnService entitySpawnService,
            IConnectionManager connectionManager,
            WorldContext worldContext)
        {
            this.entityPersistence = entityPersistence;
            this.residencyService = residencyService;
            this.sessionManager = sessionManager;
            this.entitySpawnService = entitySpawnService;
            this.connectionManager = connectionManager;
            this.worldContext = worldContext;
        }

        #region Methods
        public async Task Handle(
            UnloadSessionCommand command)
        {
            // Resolve live gameplay session first to know which room they are occupying
            var playerInstanceId = sessionManager.Get(command.UserID);
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

            // Sever this specific connection from SignalR updates and clear it from our tracking state
            await connectionManager.Ungroup(command.ConnectionID, transform.RoomSpatialID);
            connectionManager.Remove(command.UserID, command.ConnectionID);

            // Now that the dead pipe is gone, check if they are still browsing on another window or device
            if (connectionManager.HasConnections(command.UserID))
                return; 

            // Freeze active engine loops first
            entitySpawnService.Deactivate(player);

            // Save frozen data snapshot to cold storage second
            await entityPersistence.SaveManyAsync(new List<EntityInstance>() { player });

            // Clear tracking context variables and release the room residency lease
            await residencyService.LeaveRoomAsync(transform.RoomSpatialID, player.ID);
            sessionManager.Remove(command.UserID);
        }
        #endregion
    }
}