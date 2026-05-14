using Application.Interfaces.Factory;

namespace Application.Services.WorldService
{
    public class WorldExpansionService
    {
        #region Attributes
        private readonly IRoomSpatialFactory roomSpatialFactory;
        private readonly InitializationService initializationService;
        #endregion

        #region Properties
        #endregion

        public WorldExpansionService(
            IRoomSpatialFactory roomSpatialFactory,
            InitializationService initializationService)
        {
            this.roomSpatialFactory = roomSpatialFactory;
            this.initializationService = initializationService;
        }

        #region Methods
        public T Expand<T>(T worldGraph)
            where T : WorldGraph
        {
            var queue =
                new Queue<PendingRoomInitialization>(
                    worldGraph.PendingRooms);

            worldGraph.PendingRooms.Clear();

            while (queue.Count > 0)
            {
                var pending = queue.Dequeue();

                // Create room instance
                var room = roomSpatialFactory.Create(
                    definitionId: pending.RoomDefinitionID,
                    instanceId: pending.RoomSpatialID,
                    ownerId: null);

                worldGraph.Rooms.Add(room);

                // Initialize sub room
                var subWorldGraph =
                    initializationService.InitializeRoomEnvironment(
                        roomSpatialId: room.ID,
                        roomDefinitionId: room.DefinitionID);

                worldGraph.Entities.AddRange(subWorldGraph.Entities);
                worldGraph.Rooms.AddRange(subWorldGraph.Rooms);

                foreach (var next in subWorldGraph.PendingRooms)
                {
                    worldGraph.PendingRooms.Add(next);
                    queue.Enqueue(next);
                }
            }

            return worldGraph;
        }
        #endregion
    }
}