using Application.Interfaces.Realtime;
using Application.Interfaces.Realtime.Events;
using Application.Interfaces.Realtime.Events.Game;
using AutoMapper;
using Contract.DTO.Common;
using Contract.DTO.Feature.Game.Response;
using Contract.DTO.Runtime.EntityDomain;

namespace Infrastructure.Realtime.Events.Game
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
                            new EntitySpawnedDTO
                            {
                                EntityInstance = mapper.Map<EntityInstanceDTO>(e.Entity),
                                Direction = mapper.Map<Vector2DTO>(e.Direction),
                            });
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