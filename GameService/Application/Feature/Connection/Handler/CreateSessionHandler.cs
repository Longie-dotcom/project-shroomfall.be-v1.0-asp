using Application.Feature.Abstraction;
using Application.Feature.Connection.Command;
using Application.Service.WorldService;
using Application.Service.WorldService.Creation;

namespace Application.Feature.Connection.Handler
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

            initializationService.InitializeRoom(
                roomDefinitionId: dto.RoomDefinitionID,
                roomSpatialId: $"PLAYER_ROOM_{command.UserID}_{Guid.NewGuid():N}",
                userId: command.UserID,
                lifecyclePolicy: RoomLifecyclePolicy.Persistent);
        }
        #endregion
    }
}