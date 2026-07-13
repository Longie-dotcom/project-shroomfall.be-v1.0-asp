using Application.Interfaces.Realtime;
using Application.Interfaces.Realtime.Events;
using Application.Interfaces.Realtime.Events.Game;
using Contract.DTO.Feature.Game.Response;

namespace Infrastructure.Realtime.Events.Game
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
            if (@event is not EntityAppearanceChangedEvent changed)
                return;

            await publisher.SendPlayerAppearanceChanged(
                changed.RoomSpatialID,
                new EntityAppearanceChangedDTO()
                {
                    EntityInstanceID = changed.EntityInstanceID,
                    Appearance = changed.Appearance,
                });
        }
        #endregion
    }
}