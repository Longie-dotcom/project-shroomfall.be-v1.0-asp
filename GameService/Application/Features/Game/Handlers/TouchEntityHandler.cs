using Application.Coordinator;
using Application.Features.Abstraction;
using Application.Features.Game.Commands;
using Application.Interfaces.Security;
using AutoMapper;
using Contract.DTO.Connection;
using Contract.DTO.Runtime;
using Domain.DomainException;
using Domain.Shared;

namespace Application.Features.Game.Handlers
{
    public class TouchEntityHandler : IHandler<TouchEntityCommand, RoomSnapshotDTO>
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly PlayerCoordinator playerCoordinator;
        private readonly ISessionManager sessionManager;
        #endregion

        #region Properties
        #endregion

        public TouchEntityHandler(
            IMapper mapper,
            PlayerCoordinator playerCoordinator,
            ISessionManager sessionManager)
        {
            this.mapper = mapper;
            this.playerCoordinator = playerCoordinator;
            this.sessionManager = sessionManager;
        }

        #region Methods
        public async Task<RoomSnapshotDTO> Handle(
            TouchEntityCommand command)
        {
            var userId = command.UserID;
            var touchedEntityInstanceId = command.TouchedEntityInstanceID;

            // Get session
            var playerInstanceId = sessionManager.Get(userId);
            if (playerInstanceId == null)
                throw new InternalException(
                    ResponseCode.ChnageRoom_SessionNotFound,
                    $"Session was not found when changed room, user with user ID: {userId}");

            // Change player to other room and re grouping
            var snapshot = await playerCoordinator.PlayerTouchEntity(
                playerInstanceId,
                touchedEntityInstanceId);

            // Rebuild new room snapshot
            var snapshotDto = new RoomSnapshotDTO()
            {
                RoomData = mapper.Map<RoomRuntimeDTO>(snapshot.Room)
            };

            snapshotDto.RoomData.Entities = mapper.Map<List<EntityRuntimeDTO>>(snapshot.Entities);

            return snapshotDto;
        }
        #endregion
    }
}