using Application.Coordinator;
using Application.DTO.Connection;
using Application.DTO.Runtime;
using Application.Features.Abstraction;
using Application.Interfaces.Security;
using AutoMapper;
using Domain.DomainException;
using Domain.Shared;

namespace Application.Features.Connection.Handlers
{
    public class ChangeRoomHandler : IHandler<ChangeRoomCommand, RoomSnapshotDTO>
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly PlayerCoordinator playerCoordinator;
        private readonly ISessionManager sessionManager;
        #endregion

        #region Properties
        #endregion

        public ChangeRoomHandler(
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
            ChangeRoomCommand command)
        {
            var userId = command.UserID;
            var newRoomSpatialId = command.NewRoomSpatailID;

            // Get session
            var playerInstanceId = sessionManager.Get(userId);
            if (playerInstanceId == null)
                throw new InternalException(
                    ResponseCode.ChnageRoom_SessionNotFound,
                    $"Session was not found when changed room, user with user ID: {userId}");

            // Change player to other room and re grouping
            var snapshot = await playerCoordinator.PlayerChangeRoom(
                playerInstanceId,
                newRoomSpatialId);

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