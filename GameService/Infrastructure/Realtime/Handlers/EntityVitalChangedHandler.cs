using Application.Events.Abstraction;
using Application.Events.Event;
using Application.Interfaces.Realtime;
using Contract.DTO.Game; // Assuming your real-time outgoing DTOs reside here

namespace Infrastructure.Realtime.Handlers
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

            // Broadcast to the entire room so all nearby players see the health/mana bar update
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