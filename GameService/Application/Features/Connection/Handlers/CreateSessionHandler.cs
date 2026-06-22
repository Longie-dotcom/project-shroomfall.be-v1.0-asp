using Application.Features.Abstraction;
using Application.Features.Connection.Commands;
using Application.Persistence;
using Application.Services.WorldService;

namespace Application.Features.Connection.Handlers
{
    public class CreateSessionHandler : IHandler<CreateSessionCommand>
    {
        #region Attributes
        private readonly InitializationService initializationService;
        private readonly SnapshotPersistence snapshotPersistence;
        #endregion

        #region Properties
        #endregion

        public CreateSessionHandler(
            InitializationService initializationService,
            SnapshotPersistence snapshotPersistence)
        {
            this.initializationService = initializationService;
            this.snapshotPersistence = snapshotPersistence;
        }

        #region Methods
        public async Task Handle(
            CreateSessionCommand command)
        {
            var dto = command.DTO;

            // Generate IDs
            var roomSpatialId = $"PLAYER_ROOM_{command.UserID}_{Guid.NewGuid():N}";
            var playerInstanceId = $"{command.UserID}_{Guid.NewGuid():N}";

            // Create room snapshot
            var snapshot = initializationService.InitializeRoom(
                roomDefinitionId: dto.RoomDefinitionID,
                roomSpatialId: roomSpatialId,
                playerDefinitionId: dto.PlayerDefinitionID,
                playerInstanceId: playerInstanceId);

            // Persist snapshot
            await snapshotPersistence.SaveRoomSnapshotAsync(snapshot.room);
        }
        #endregion
    }
}