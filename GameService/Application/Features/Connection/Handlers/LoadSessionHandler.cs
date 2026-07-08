using Application.Features.Abstraction;
using Application.Features.Connection.Commands;
using Application.Interfaces.Realtime.Managers;
using Application.Persistence;
using Application.Services.WorldService;
using AutoMapper;
using Contract.DTO.Connection;
using Contract.DTO.Domain.Runtime;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using ResponseCode;

namespace Application.Features.Connection.Handlers
{
    public class LoadSessionHandler : IHandler<LoadSessionCommand, SaveGameDTO>
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly EntityPersistence entityPersistence;
        private readonly ISessionManager sessionManager;
        private readonly RoomMigrationService roomMigrationService;
        #endregion

        #region Properties
        #endregion

        public LoadSessionHandler(
            IMapper mapper,
            EntityPersistence entityPersistence,
            ISessionManager sessionManager,
            RoomMigrationService roomMigrationService)
        {
            this.mapper = mapper;
            this.entityPersistence = entityPersistence;
            this.sessionManager = sessionManager;
            this.roomMigrationService = roomMigrationService;
        }

        #region Methods
        public async Task<SaveGameDTO> Handle(
            LoadSessionCommand command)
        {
            var dto = command.DTO;

            // Prevent duplicate session
            if (sessionManager.Get(command.UserID) != null)
                throw new BadRequest(
                    ApplicationCode.ConnectionHandlerCode.LoadSessionAlreadyExisted,
                    $"Session of user with user ID: {command.UserID} already existed with player instance ID: {dto.PlayerInstanceID}");

            // Reload player instance from cold persistence storage
            var player = await entityPersistence.LoadEntityAsync(dto.PlayerInstanceID);
            if (player == null)
                throw new InternalException(
                    ApplicationCode.ConnectionHandlerCode.LoadSessionPlayerNotFoundInPersistence,
                    $"Player instance on load with instance ID: {dto.PlayerInstanceID} is not found");

            // Validate ownership existence
            var ownership = player.GetComponent<OwnershipInstance>();
            if (ownership == null)
                throw new InternalException(
                    ApplicationCode.ConnectionHandlerCode.LoadSessionOwnershipMissing,
                    $"Player instance {player.ID} is missing OwnershipInstance component during session load");

            // Authorize session
            if (command.UserID != ownership.UserID)
                throw new Unauthorized(
                    ApplicationCode.ConnectionHandlerCode.LoadSessionUnauthorizedPlayer,
                    $"Player session is unauthorized");

            // Migrate player safely using calculated blueprint rules
            var snapshot = await roomMigrationService.EnterRoomAsync(
                player: player,
                destinationRoomId: ownership.PersonalRoomID,
                isInitialLogin: true);

            sessionManager.Add(command.UserID, player.ID);

            // Rebuild save game snapshot
            var saveGame = BuildSaveGame(player, snapshot);

            return saveGame;
        }

        private SaveGameDTO BuildSaveGame(
            EntityInstance player,
            RoomSnapshotDTO snapshot)
        {
            var saveGame = new SaveGameDTO()
            {
                PlayerData = mapper.Map<EntityInstanceDTO>(player),
                RoomData = snapshot.RoomData
            };

            return saveGame;
        }
        #endregion
    }
}