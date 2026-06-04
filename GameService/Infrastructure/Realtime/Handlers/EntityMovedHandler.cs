using Application.Events.Abstraction;
using Application.Events.Event;
using Application.Interfaces.Realtime;
using Contract.DTO.Game;

namespace Infrastructure.Realtime.Handlers
{
    public class EntityMovedHandler : IEventHandler
    {
        #region Attributes
        private readonly IRealtimePublisher publisher;
        #endregion

        #region Properties
        #endregion

        public EntityMovedHandler(
            IRealtimePublisher publisher)
        {
            this.publisher = publisher;
        }

        #region Methods
        public async Task Handle(
            IEvent @event)
        {
            if (@event is not EntityMovedEvent moved)
                return;

            await publisher.SendEntityMoved(
                moved.RoomSpatialID,
                new EntityMovedDTO()
                {
                    X = moved.Position.X,
                    Y = moved.Position.Y,
                });
        }
        #endregion
    }
}