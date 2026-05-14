using Application.Coordinator;
using Application.DTO.Connection;
using Application.DTO.Runtime;
using Application.Features.Abstraction;
using Application.Features.Connection.Commands;
using AutoMapper;

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
            var playerInstance = await playerCoordinator.CreateNewPlayer(
                dto.PlayerDefinitionID,
                dto.RoomDefinitionID,
                command.UserID);

            // Mapping and return
            return new ExistedSessionEntryDTO()
            {
                PlayerInstanceID = playerInstance.ID,
                PlayerAppearance = mapper.Map<PlayerAppearanceRuntimeDTO>(playerInstance.PlayerAppearance)
            };
        }
        #endregion
    }
}