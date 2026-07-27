using Application.Services.WorldService.Creation;

namespace Application.Services.WorldService.Persistence
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
        public async Task<RoomInstance?> LoadRoomInstanceAsync(
            string roomSpatialId)
        {
            var room = await roomPersistence.LoadAsync(roomSpatialId);
            if (room == null)
                return null;

            var entities = await entityPersistence.LoadByRoomAsync(roomSpatialId);

            return new RoomInstance
            {
                Room = room,
                Entities = entities
            };
        }

        public async Task SaveRoomInstanceAsync(RoomInstance instance)
        {
            await roomPersistence.SaveAsync(instance.Room);

            if (instance.Entities != null && instance.Entities.Any())
            {
                await entityPersistence.SaveManyAsync(instance.Entities);
                await entityPersistence.DeleteMissingEntitiesInRoomAsync(instance.Room.ID, instance.Entities.Select(e => e.ID).ToList());
            }
        }
        #endregion
    }
}