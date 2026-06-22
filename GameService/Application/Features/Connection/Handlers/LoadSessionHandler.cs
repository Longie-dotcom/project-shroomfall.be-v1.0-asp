using Application.Context;
using Application.Features.Abstraction;
using Application.Features.Connection.Commands;
using Application.Interfaces.Realtime.Managers;
using Application.Persistence;
using Application.Services.WorldService;
using AutoMapper;
using Contract.DTO.Connection;
using Contract.DTO.Domain.Runtime;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;

namespace Application.Features.Connection.Handlers
{
    public class LoadSessionHandler : IHandler<LoadSessionCommand, SaveGameDTO>
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly CollisionService collisionService;
        private readonly EntityPersistence entityPersistence;
        private readonly ResidencyService residencyService;
        private readonly PlayerContext playerContext;
        private readonly IConnectionManager connectionManager;
        private readonly ISessionManager sessionManager;
        private readonly EntitySpawnService entitySpawnService;
        #endregion

        #region Properties
        #endregion

        public LoadSessionHandler(
            IMapper mapper,
            CollisionService collisionService,
            EntityPersistence entityPersistence,
            ResidencyService residencyService,
            PlayerContext playerContext,
            IConnectionManager connectionManager,
            ISessionManager sessionManager,
            EntitySpawnService entitySpawnService)
        {
            this.mapper = mapper;
            this.collisionService = collisionService;
            this.entityPersistence = entityPersistence;
            this.residencyService = residencyService;
            this.playerContext = playerContext;
            this.connectionManager = connectionManager;
            this.sessionManager = sessionManager;
            this.entitySpawnService = entitySpawnService;
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

            // Retrieve room definition
            var transform = player.GetComponent<TransformInstance>();
            if (transform == null)
                throw new InternalException(
                    ApplicationCode.ConnectionHandlerCode.LoadSessionTransformMissing,
                    $"Player instance {player.ID} is missing TransformInstance component during session load");

            // Ensure room residency
            var snapshot = await residencyService.EnsureRoomLoaded(transform.RoomSpatialID);
            if (snapshot == null)
                throw new InternalException(
                    ApplicationCode.ConnectionHandlerCode.LoadSessionRoomNotFound,
                    $"Room with spatial ID: {transform.RoomSpatialID} was not found during session load");

            // Validate spawn
            collisionService.SpawnAtNearestValidPosition(
                entity: player,
                roomDefinitionId: snapshot.Room.DefinitionID,
                roomSpatialId: snapshot.Room.ID,
                targetPosition: transform.Position,
                targetLayerZ: transform.LayerZ,
                null);

            // Active the player
            entitySpawnService.Activate(player);

            // Mutate runtime
            await ApplySessionRuleAsync(player, command.UserID);

            // Rebuild save game snapshot
            var saveGame = BuildSaveGame(player, snapshot);

            return saveGame;
        }

        private async Task ApplySessionRuleAsync(
            EntityInstance player,
            string userId)
        {
            var transform = player.GetComponent<TransformInstance>();
            if (transform == null) return;

            // Bind core simulation contexts
            playerContext.JoinRoom(transform.RoomSpatialID, player.ID);
            residencyService.MarkRoomHot(transform.RoomSpatialID);
            sessionManager.Add(userId, player.ID);

            // Fetch all connections pre-registered 
            var activeConnections = connectionManager.Get(userId);

            // Ensure the client didn't somehow bypass the socket connection step
            if (activeConnections.Count == 0)
                return;

            // Batch-assign all active devices/tabs to this room stream
            foreach (var connectionId in activeConnections)
            {
                await connectionManager.Group(connectionId, transform.RoomSpatialID);
            }
        }

        private SaveGameDTO BuildSaveGame(
            EntityInstance player,
            RoomSnapshot snapshot)
        {
            var saveGame = new SaveGameDTO()
            {
                PlayerData = mapper.Map<EntityInstanceDTO>(player),
                RoomData = mapper.Map<RoomRuntimeDTO>(snapshot.Room)
            };

            saveGame.RoomData.Entities = mapper.Map<List<EntityInstanceDTO>>(snapshot.Entities);

            return saveGame;
        }
        #endregion
    }
}