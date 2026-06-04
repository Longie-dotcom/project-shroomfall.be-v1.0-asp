using Application.Events.Abstraction;
using Application.Events.Event;
using Application.Interfaces.Realtime;
using Contract.DTO.Game;

namespace Infrastructure.Realtime.Handlers
{
    public class PlayerAppearanceChangedHandler : IEventHandler
    {
        #region Attributes
        private readonly IRealtimePublisher publisher;
        #endregion

        #region Properties
        #endregion

        public PlayerAppearanceChangedHandler(
            IRealtimePublisher publisher)
        {
            this.publisher = publisher;
        }

        #region Methods
        public async Task Handle(
            IEvent @event)
        {
            if (@event is not PlayerAppearanceChangedEvent changed)
                return;

            await publisher.SendPlayerAppearanceChanged(
                changed.RoomSpatialID,
                new PlayerAppearanceChangedDTO()
                {
                    EntityInstanceID = changed.EntityInstanceID,
                    Appearance = changed.Appearance,
                });
        }
        #endregion
    }
}