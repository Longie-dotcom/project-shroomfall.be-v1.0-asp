using Application.Coordinator;
using Application.Features.Abstraction;
using Application.Features.Connection.Commands;
using Application.Interfaces.Security;
using AutoMapper;
using Contract.DTO.Connection;
using Contract.DTO.Runtime;
using Domain.DomainException;
using Domain.Shared;

namespace Application.Features.Connection.Handlers
{
    public class LoadSessionHandler : IHandler<LoadSessionCommand, SaveGameDTO>
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly PlayerCoordinator playerCoordinator;
        private readonly ISessionManager sessionManager; 
        #endregion

        #region Properties
        #endregion

        public LoadSessionHandler(
            IMapper mapper,
            PlayerCoordinator playerCoordinator,
            ISessionManager sessionManager)
        {
            this.mapper = mapper;
            this.playerCoordinator = playerCoordinator;
            this.sessionManager = sessionManager;
        }

        #region Methods
        public async Task<SaveGameDTO> Handle(
            LoadSessionCommand command)
        {
            var dto = command.DTO;

            // Prevent duplicate session
            if (sessionManager.Get(command.UserID) != null)
                throw new BadRequest(
                    ResponseCode.LoadSession_SessionAlreadyExisted,
                    $"Session of user with user ID: {command.UserID} already existed with player instance ID: {dto.PlayerInstanceID}");

            // Reload player instance (old save)
            var (player, snapshot) = await playerCoordinator.LoadPlayer(dto.PlayerInstanceID, command.UserID);

            // Register session
            sessionManager.Add(command.UserID, dto.PlayerInstanceID);

            // Rebuild save game snapshot
            var saveGame = new SaveGameDTO()
            {
                PlayerData = mapper.Map<PlayerRuntimeDTO>(player),
                RoomData = mapper.Map<RoomRuntimeDTO>(snapshot.Room)
            };

            saveGame.RoomData.Entities = mapper.Map<List<EntityRuntimeDTO>>(snapshot.Entities);

            return saveGame;
        }
        #endregion
    }
}