using Application.Interfaces.Realtime;
using Application.Interfaces.Realtime.Events;
using Application.Interfaces.Realtime.Events.Design;
using Contract.DTO.Feature.Design.Response;

namespace Infrastructure.Realtime.Events.Design
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
                new UpdateDefinitionNotificationDTO()
                {
                    Key = e.Key,
                    Version = e.Version,
                });
        }
        #endregion
    }
}