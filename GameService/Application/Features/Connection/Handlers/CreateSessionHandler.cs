using Application.Features.Abstraction;
using Application.Features.Connection.Commands;
using Application.Services.Abstraction.WorldService;

namespace Application.Features.Connection.Handlers
{
    public class CreateSessionHandler : IHandler<CreateSessionCommand>
    {
        #region Attributes
        private readonly IOrchestratorService orchestratorService;
        #endregion

        #region Properties
        #endregion

        public CreateSessionHandler(
            IOrchestratorService orchestratorService)
        {
            this.orchestratorService = orchestratorService;
        }

        #region Methods
        public async Task Handle(
            CreateSessionCommand command)
        {
            var dto = command.DTO;

            // Create new player instance (new save)
            await orchestratorService.SpawnNewPlayer(
                dto.PlayerDefinitionID,
                dto.RoomDefinitionID,
                command.UserID);
        }
        #endregion
    }
}