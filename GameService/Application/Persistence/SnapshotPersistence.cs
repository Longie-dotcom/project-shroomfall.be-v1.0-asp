using Application.Services.WorldService;

namespace Application.Persistence
{
    public class SnapshotPersistence
    {
        #region Attributes
        private readonly EntityPersistence entityPersistence;
        private readonly RoomPersistence roomPersistence;
        #endregion

        #region Properties
        #endregion

        public SnapshotPersistence(
            EntityPersistence entityPersistence,
            RoomPersistence roomPersistence)
        {
            this.entityPersistence = entityPersistence;
            this.roomPersistence = roomPersistence;
        }

        #region Methods
        public async Task<RoomSnapshot?> LoadRoomSnapshotAsync(
            string roomSpatialId)
        {
            var room = await roomPersistence.LoadAsync(roomSpatialId);
            if (room == null)
                return null;

            var entities = await entityPersistence.LoadByRoomAsync(roomSpatialId);

            return new RoomSnapshot
            {
                Room = room,
                Entities = entities
            };
        }

        public async Task SaveRoomSnapshotAsync(
            RoomSnapshot snapshot)
        {
            await roomPersistence.SaveAsync(snapshot.Room);

            await entityPersistence.SaveManyAsync(snapshot.Entities);
        }
        #endregion
    }
}