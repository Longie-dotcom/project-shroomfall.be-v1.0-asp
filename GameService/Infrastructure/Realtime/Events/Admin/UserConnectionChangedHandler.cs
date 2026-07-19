using Application.Interfaces.Realtime;
using Application.Interfaces.Realtime.Events;
using Application.Interfaces.Realtime.Events.Admin;
using Contract.DTO.Feature.Admin.Response;

namespace Infrastructure.Realtime.Events.Admin
{
    public class UserConnectionChangedHandler : IEventHandler
    {
        #region Attributes
        private readonly IRealtimePublisher publisher;
        #endregion

        #region Properties
        #endregion

        public UserConnectionChangedHandler(
            IRealtimePublisher publisher)
        {
            this.publisher = publisher;
        }

        #region Methods
        public async Task Handle(
            IEvent @event)
        {
            if (@event is not UserConnectionChangedEvent e)
                return;

            await publisher.SendUserConnectionChanged(
                new UserConnectionChangedDTO()
                {
                    UserID = e.UserID,
                    ConnectionID = e.ConnectionID,
                });
        }
        #endregion
    }
}