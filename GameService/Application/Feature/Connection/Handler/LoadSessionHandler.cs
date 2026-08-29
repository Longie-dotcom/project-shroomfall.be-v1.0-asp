using Application.Feature.Abstraction;
using Application.Feature.Connection.Command;
using Application.Interface.Realtime.Managers;
using Application.Service.WorldService;
using AutoMapper;
using Contract.DTO.Feature.Connection.Response;
using Contract.DTO.Runtime.EntityDomain;
using Contract.DTO.Runtime.WorldDomain;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using ResponseCode;

namespace Application.Feature.Connection.Handler
{
    public class LoadSessionHandler : IHandler<LoadSessionCommand, SaveGameDTO>
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly ISessionManager sessionManager;
        private readonly RoomMigrationService roomMigrationService;
        private readonly ResidencyService residencyService;
        #endregion

        #region Properties
        #endregion

        public LoadSessionHandler(
            IMapper mapper,
            ISessionManager sessionManager,
            RoomMigrationService roomMigrationService,
            ResidencyService residencyService)
        {
            this.mapper = mapper;
            this.sessionManager = sessionManager;
            this.roomMigrationService = roomMigrationService;
            this.residencyService = residencyService;
        }

        #region Methods
        public async Task<SaveGameDTO> Handle(
            LoadSessionCommand command)
        {
            var dto = command.DTO;

            // Ensure Player is in RAM
            var player = await residencyService.EnsurePlayerExisted(dto.PlayerInstanceID);

            // Validate ownership
            var ownership = player.GetComponent<OwnershipInstance>();
            if (command.UserID != ownership!.UserID)
                throw new Unauthorized(
                    ApplicationCode.ConnectionHandlerCode.LoadSessionUnauthorizedPlayer,
                    $"Player session is unauthorized");

            // Migrate player to their Personal Room (Handles spawn points, network groups, etc.)
            var snapshot = await roomMigrationService.EnterRoomAsync(
                player: player,
                destinationRoomId: ownership.PersonalRoomID);

            // Add session
            sessionManager.Add(command.UserID, player.ID);

            return BuildSaveGame(player, snapshot);
        }

        private SaveGameDTO BuildSaveGame(
            EntityInstance player,
            RoomInstanceDTO room)
        {
            var saveGame = new SaveGameDTO()
            {
                PlayerData = mapper.Map<EntityInstanceDTO>(player),
                RoomData = room
            };

            return saveGame;
        }
        #endregion
    }
}