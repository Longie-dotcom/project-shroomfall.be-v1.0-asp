using Application.Coordinator;
using Application.Features.Abstraction;
using Application.Features.Connection.Commands;
using AutoMapper;
using Contract.DTO.Connection;
using Contract.DTO.Runtime;

namespace Application.Features.Connection.Handlers
{
    public class CreateSessionHandler : IHandler<CreateSessionCommand, ExistedSessionEntryDTO>
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly PlayerCoordinator playerCoordinator;
        #endregion

        #region Properties
        #endregion

        public CreateSessionHandler(
            IMapper mapper,
            PlayerCoordinator playerCoordinator)
        {
            this.mapper = mapper;
            this.playerCoordinator = playerCoordinator;
        }

        #region Methods
        public async Task<ExistedSessionEntryDTO> Handle(
            CreateSessionCommand command)
        {
            var dto = command.DTO;

            // Create new player instance (new save)
            var playerInstance = await playerCoordinator.CreatePlayer(
                dto.PlayerDefinitionID,
                dto.RoomDefinitionID,
                command.UserID);

            // Mapping and return
            return new ExistedSessionEntryDTO()
            {
                PlayerInstanceID = playerInstance.ID,
                PlayerAppearance = mapper.Map<AppearanceRuntimeDTO>(playerInstance.Appearance)
            };
        }
        #endregion
    }
}