using Application.Features.Abstraction;
using Application.Features.Connection.Commands;
using Application.Services.WorldService;

namespace Application.Features.Connection.Handlers
{
    public class CreateSessionHandler : IHandler<CreateSessionCommand>
    {
        #region Attributes
        private readonly InitializationService initializationService;
        #endregion

        #region Properties
        #endregion

        public CreateSessionHandler(
            InitializationService initializationService)
        {
            this.initializationService = initializationService;
        }

        #region Methods
        public async Task Handle(
            CreateSessionCommand command)
        {
            var dto = command.DTO;

            // Create room snapshot
            initializationService.InitializeRoom(
                roomDefinitionId: dto.RoomDefinitionID,
                roomSpatialId: $"PLAYER_ROOM_{command.UserID}_{Guid.NewGuid():N}",
                userId: command.UserID,
                lifecyclePolicy: RoomLifecyclePolicy.Persistent);
        }
        #endregion
    }
}