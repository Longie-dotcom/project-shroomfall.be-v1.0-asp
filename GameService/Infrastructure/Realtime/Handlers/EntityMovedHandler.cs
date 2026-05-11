using Application.Events.Abstraction;
using Application.Events.Event;
using Application.Interfaces.Realtime;

namespace Infrastructure.Realtime.Handlers
{
    public class EntityMovedHandler : IEventHandler
    {
        #region Attributes
        private readonly IRealtimePublisher publisher;
        #endregion

        #region Properties
        #endregion

        public EntityMovedHandler(IRealtimePublisher publisher)
        {
            this.publisher = publisher;
        }

        #region Methods
        public async Task Handle(IEvent @event)
        {
            if (@event is not EntityMovedEvent moved)
                return;

            await publisher.SendEntityMoved(
                moved.RoomID,
                new
                {
                    entityId = moved.EntityID,
                    x = moved.Position.X,
                    y = moved.Position.Y
                });
        }
        #endregion
    }
}