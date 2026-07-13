using Application.Interfaces.Realtime;
using Application.Interfaces.Realtime.Events;
using Application.Interfaces.Realtime.Events.Game;
using Contract.DTO.Feature.Game.Response;

namespace Infrastructure.Realtime.Events.Game
{
    public class EntityVitalChangedHandler : IEventHandler
    {
        #region Attributes
        private readonly IRealtimePublisher publisher;
        #endregion

        #region Properties
        #endregion

        public EntityVitalChangedHandler(
            IRealtimePublisher publisher)
        {
            this.publisher = publisher;
        }

        #region Methods
        public async Task Handle(
            IEvent @event)
        {
            if (@event is not EntityVitalChangedEvent vitalChanged)
                return;

            await publisher.SendEntityVitalChanged(
                vitalChanged.RoomSpatialID,
                new EntityVitalChangedDTO()
                {
                    EntityInstanceID = vitalChanged.EntityInstanceID,
                    AttributeType = vitalChanged.AttributeType,
                    NewValue = vitalChanged.NewValue,
                });
        }
        #endregion
    }
}