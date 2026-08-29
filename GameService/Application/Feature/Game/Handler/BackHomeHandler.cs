using Application.Feature.Abstraction;
using Application.Feature.Game.Command;
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

namespace Application.Feature.Game.Handler
{
    public class BackHomeHandler : IHandler<BackHomeCommand, SaveGameDTO>
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly ISessionManager sessionManager;
        private readonly WorldContext worldContext;
        private readonly RoomMigrationService roomMigrationService;
        #endregion

        #region Properties
        #endregion

        public BackHomeHandler(
            IMapper mapper,
            ISessionManager sessionManager,
            WorldContext worldContext,
            RoomMigrationService roomMigrationService)
        {
            this.mapper = mapper;
            this.sessionManager = sessionManager;
            this.worldContext = worldContext;
            this.roomMigrationService = roomMigrationService;
        }

        #region Methods
        public async Task<SaveGameDTO> Handle(
            BackHomeCommand command)
        {
            // Validate player session
            var playerInstanceId = sessionManager.Get(command.UserID);
            if (playerInstanceId == null)
                throw new InternalException(
                    ApplicationCode.GameHandlerCode.BackHomeSessionNotFound,
                    $"Session missing for user '{command.UserID}'.");

            // Validate runtime player
            var player = worldContext.GetEntity(playerInstanceId);
            if (player == null)
                throw new InternalException(
                    ApplicationCode.GameHandlerCode.BackHomePlayerInstanceNotFound,
                    $"Player runtime instance '{playerInstanceId}' not found.");

            // Resolve owned personal room
            var ownership = player.GetComponent<OwnershipInstance>();
            if (ownership == null)
                throw new InternalException(
                    ApplicationCode.GameHandlerCode.BackHomeOwnershipInstanceNotFound,
                    $"Player runtime instance '{playerInstanceId}' has no Ownership Instance.");

            // Migrate player safely using calculated blueprint rules
            var roomInstance = await roomMigrationService.EnterRoomAsync(
                player: player,
                destinationRoomId: ownership.PersonalRoomID);

            return BuildSaveGame(player, roomInstance);
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