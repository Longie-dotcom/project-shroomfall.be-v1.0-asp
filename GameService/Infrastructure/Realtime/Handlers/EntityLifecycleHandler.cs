using Application.Events.Abstraction;
using Application.Events.Event;
using Application.Interfaces.Realtime;
using Application.Services.Abstraction.OtherService;

namespace Infrastructure.Realtime.Handlers
{
    public class EntityLifecycleHandler : IEventHandler
    {
        #region Attributes
        private readonly ISnapshotService snapshotService;
        private readonly IRealtimePublisher publisher;
        #endregion

        #region Properties
        #endregion

        public EntityLifecycleHandler(
            ISnapshotService snapshotService,
            IRealtimePublisher publisher)
        {
            this.snapshotService = snapshotService;
            this.publisher = publisher;
        }

        #region Methods
        public async Task Handle(IEvent @event)
        {
            if (@event is not EntityLifecycleEvent e)
                return;

            switch (e.Type)
            {
                case EntityLifecycleType.Spawn:
                    {
                        var entity = snapshotService.BuildEntity(e.EntityID);

                        await publisher.SendEntitySpawned(
                            e.RoomID,
                            entity);

                        break;
                    }

                case EntityLifecycleType.Despawn:
                    {
                        await publisher.SendEntityDespawned(
                            e.RoomID,
                            e.EntityID);

                        break;
                    }
            }
        }
        #endregion
    }
}