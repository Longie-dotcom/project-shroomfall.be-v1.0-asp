using Application.Events.Abstraction;
using Application.Events.Event;
using Application.Interfaces.Realtime;

namespace Infrastructure.Realtime.Handlers
{
    public class DefinitionUpdatedHandler : IEventHandler
    {
        #region Attributes
        private readonly IRealtimePublisher publisher;
        #endregion

        #region Properties
        #endregion

        public DefinitionUpdatedHandler(
            IRealtimePublisher publisher)
        {
            this.publisher = publisher;
        }

        #region Methods
        public async Task Handle(
            IEvent @event)
        {
            if (@event is not DefinitionUpdatedEvent e)
                return;

            await publisher.SendDefinitionUpdated(
                e.Key,
                e.Version
            );
        }
        #endregion
    }
}