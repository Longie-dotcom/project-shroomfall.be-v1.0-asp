using Application.DTO.Runtime;
using Application.Events.Abstraction;
using Application.Events.Event;
using Application.Interfaces.Realtime;
using AutoMapper;

namespace Infrastructure.Realtime.Handlers
{
    public class EntityLifecycleHandler : IEventHandler
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly IRealtimePublisher publisher;
        #endregion

        #region Properties
        #endregion

        public EntityLifecycleHandler(
            IMapper mapper,
            IRealtimePublisher publisher)
        {
            this.mapper = mapper;
            this.publisher = publisher;
        }

        #region Methods
        public async Task Handle(
            IEvent @event)
        {
            if (@event is not EntityLifecycleEvent e)
                return;

            switch (e.Type)
            {
                case EntityLifecycleType.Spawn:
                    {
                        await publisher.SendEntitySpawned(
                            e.RoomSpatialID,
                            mapper.Map<EntityRuntimeDTO>(e.Entity));
                        break;
                    }

                case EntityLifecycleType.Despawn:
                    {
                        await publisher.SendEntityDespawned(
                            e.RoomSpatialID,
                            e.Entity.ID);
                        break;
                    }
            }
        }
        #endregion
    }
}