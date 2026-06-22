using Application.Interfaces.Realtime;
using Application.Interfaces.Realtime.Events;
using Application.Interfaces.Realtime.Events.Admin;
using Contract.DTO.Admin;

namespace Infrastructure.Realtime.Events.Admin
{
    public class UserSessionChangedHandler : IEventHandler
    {
        #region Attributes
        private readonly IRealtimePublisher publisher;
        #endregion

        #region Properties
        #endregion

        public UserSessionChangedHandler(
            IRealtimePublisher publisher)
        {
            this.publisher = publisher;
        }

        #region Methods
        public async Task Handle(
            IEvent @event)
        {
            if (@event is not UserSessionChangedEvent e)
                return;

            await publisher.SendUserSessionChanged(
                new UserSessionChangedDTO()
                {
                    UserID = e.UserID,
                    PlayerInstanceID = e.PlayerInstanceID
                });
        }
        #endregion
    }
}