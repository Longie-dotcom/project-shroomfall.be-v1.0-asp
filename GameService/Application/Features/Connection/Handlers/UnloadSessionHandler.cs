using Application.Coordinator;
using Application.Features.Abstraction;
using Application.Features.Connection.Commands;
using Application.Interfaces.Realtime;
using Application.Interfaces.Security;
using Domain.DomainException;
using Domain.Shared;

namespace Application.Features.Connection.Handlers
{
    public class UnloadSessionHandler : IHandler<UnloadSessionCommand>
    {
        #region Attributes
        private readonly PlayerCoordinator playerCoordinator;
        private readonly IConnectionRegistry connectionRegistry;
        private readonly ISessionManager sessionManager;
        #endregion

        #region Properties
        #endregion

        public UnloadSessionHandler(
            PlayerCoordinator playerCoordinator,
            IConnectionRegistry connectionRegistry,
            ISessionManager sessionManager)
        {
            this.playerCoordinator = playerCoordinator;
            this.connectionRegistry = connectionRegistry;
            this.sessionManager = sessionManager;
        }

        #region Methods
        public async Task Handle(
            UnloadSessionCommand command)
        {
            var userId = command.UserID;
            var connectionId = command.ConnectionID;

            // Remove this transport connection only
            connectionRegistry.Remove(userId, connectionId);

            // Keep other connection
            if (connectionRegistry.HasConnections(userId))
                return;

            // Resolve gameplay session
            var playerInstanceId = sessionManager.Get(userId);
            if (playerInstanceId == null)
                throw new InternalException(
                    ResponseCode.UnloadSession_SessionNotFound,
                    $"User with user ID: {userId} has no session found");

            // Persisted and unload player instance (saving)
            await playerCoordinator.UnloadPlayer(playerInstanceId);

            // Cleanup gameplay session mapping
            sessionManager.Remove(userId);
        }
        #endregion
    }
}