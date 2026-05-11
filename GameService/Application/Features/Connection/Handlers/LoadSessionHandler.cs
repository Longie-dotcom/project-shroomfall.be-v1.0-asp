using Application.Features.Abstraction;
using Application.Features.Connection.Commands;
using Application.Interfaces.Security;
using Application.Services.Abstraction.WorldService;
using Domain.DomainException;
using Domain.Shared;

namespace Application.Features.Connection.Handlers
{
    public class LoadSessionHandler : IHandler<LoadSessionCommand>
    {
        #region Attributes
        private readonly IOrchestratorService orchestratorService;
        private readonly ISessionManager sessionManager; 
        #endregion

        #region Properties
        #endregion

        public LoadSessionHandler(
            IOrchestratorService orchestratorService,
            ISessionManager sessionManager)
        {
            this.orchestratorService = orchestratorService;
            this.sessionManager = sessionManager;
        }

        #region Methods
        public async Task Handle(
            LoadSessionCommand command)
        {
            var dto = command.DTO;

            // Prevent duplicate session
            if (sessionManager.Get(command.UserID) != null)
                throw new BadRequest(
                    ResponseCode.LoadSession_SessionAlreadyExisted,
                    $"Session of user with user ID: {command.UserID} already existed with player instance ID: {dto.PlayerInstanceID}");

            // Reload player instance (old save)
            await orchestratorService.LoadExistedPlayer(dto.PlayerInstanceID);

            // Register session
            sessionManager.Add(command.UserID, dto.PlayerInstanceID);
        }
        #endregion
    }
}