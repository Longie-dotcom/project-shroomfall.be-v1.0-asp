using Application.Features.Abstraction;
using Application.Features.Connection.Commands;
using Application.Interfaces.Realtime;
using Application.Interfaces.Security;
using Application.Services.Abstraction.WorldService;
using Domain.DomainException;
using Domain.Shared;

namespace Application.Features.Connection.Handlers
{
    public class UnloadSessionHandler : IHandler<UnloadSessionCommand>
    {
        #region Attributes
        private readonly IConnectionRegistry connectionRegistry;
        private readonly ISessionManager sessionManager;
        private readonly IOrchestratorService orchestratorService;
        #endregion

        #region Properties
        #endregion

        public UnloadSessionHandler(
            IConnectionRegistry connectionRegistry,
            ISessionManager sessionManager,
            IOrchestratorService orchestratorService)
        {
            this.connectionRegistry = connectionRegistry;
            this.sessionManager = sessionManager;
            this.orchestratorService = orchestratorService;
        }

        #region Methods
        public async Task Handle(
            UnloadSessionCommand command)
        {
            var userId = command.UserID;

            // Resolve session
            var playerInstanceId = sessionManager.Get(userId);
            if (playerInstanceId == null)
                throw new InternalException(
                    ResponseCode.UnloadSession_SessionNotFound,
                    $"User with user ID: {userId} has no session found");

            // Persisted and unload player instance (saving)
            await orchestratorService.UnloadExistedPlayer(playerInstanceId);

            // Clean up session
            connectionRegistry.Remove(userId);
            sessionManager.Remove(userId);
        }
        #endregion
    }
}