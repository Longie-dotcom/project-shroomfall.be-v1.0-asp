using Application.Events.Abstraction;
using Application.Events.Event;
using Application.Interfaces.Realtime;
using Application.Services.Abstraction.OtherService;

namespace Infrastructure.Realtime.Handlers
{
    public class EntityRoomChangedHandler : IEventHandler
    {
        #region Attributes
        private readonly IConnectionRegistry registry;
        private readonly IConnectionManager manager;
        private readonly ISnapshotService snapshotService;
        private readonly IRealtimePublisher publisher;
        #endregion

        #region Properties
        #endregion

        public EntityRoomChangedHandler(
            IConnectionRegistry registry,
            IConnectionManager manager,
            ISnapshotService snapshotService,
            IRealtimePublisher publisher)
        {
            this.registry = registry;
            this.manager = manager;
            this.snapshotService = snapshotService;
            this.publisher = publisher;
        }

        #region Methods
        public async Task Handle(IEvent @event)
        {
            if (@event is not EntityRoomChangedEvent e)
                return;

            var connectionId = registry.GetConnection(e.EntityID);
            if (connectionId == null)
                return;

            // 1. leave old room
            await manager.LeaveAsync(connectionId, e.OldRoomID);

            // 2. join new room
            await manager.JoinAsync(connectionId, e.NewRoomID);

            // 3. send FULL snapshot (only to that player)
            var snapshot = snapshotService.BuildSnapshot(e.EntityID);

            await publisher.SendRoomSnapshot(connectionId, snapshot);
        }
        #endregion
    }
}