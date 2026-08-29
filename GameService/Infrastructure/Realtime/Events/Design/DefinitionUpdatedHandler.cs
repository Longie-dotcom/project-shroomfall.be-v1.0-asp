using Application.Interface.Realtime;
using Application.Interface.Realtime.Events;
using Application.Interface.Realtime.Events.Design;
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