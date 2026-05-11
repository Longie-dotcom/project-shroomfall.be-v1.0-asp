using Application.Interfaces.Factory;
using Application.Services.Abstraction.WorldService;

namespace Application.Services.WorldService
{
    public class WorldExpansionService : IWorldExpansionService
    {
        #region Attributes
        private readonly IRoomSpatialFactory roomSpatialFactory;
        private readonly IInitializationService initializationService;
        #endregion

        #region Properties
        #endregion

        public WorldExpansionService(
            IRoomSpatialFactory roomSpatialFactory,
            IInitializationService initializationService)
        {
            this.roomSpatialFactory = roomSpatialFactory;
            this.initializationService = initializationService;
        }

        #region Methods
        public WorldContext Expand(WorldContext seed)
        {
            var result = new WorldContext();

            var queue = new Queue<PendingRoomInitialization>(seed.PendingRooms);

            while (queue.Count > 0)
            {
                var pending = queue.Dequeue();

                // Create room instance
                var room = roomSpatialFactory.Create(
                    definitionId: pending.RoomDefinitionID,
                    instanceId: pending.RoomSpatialID,
                    ownerId: null);

                result.Rooms.Add(room);

                // Initialize sub-context
                var subContext = initializationService.InitializeRoomEnvironment(
                    roomSpatialId: room.ID,
                    roomDefinitionId: room.DefinitionID);

                result.Entities.AddRange(subContext.Entities);

                foreach (var next in subContext.PendingRooms)
                    queue.Enqueue(next);
            }

            return result;
        }
        #endregion
    }
}