using Application.Interfaces.Realtime;
using Microsoft.AspNetCore.SignalR;
using SignalHub;

namespace Infrastructure.Realtime
{
    public class ConnectionManager : IConnectionManager
    {
        #region Attributes
        private readonly IHubContext<GameHub> hub;
        #endregion

        #region Properties
        #endregion

        public ConnectionManager(IHubContext<GameHub> hub)
        {
            this.hub = hub;
        }

        #region Methods
        public Task JoinAsync(string connectionId, string groupId)
        {
            return hub.Groups.AddToGroupAsync(connectionId, groupId);
        }

        public Task LeaveAsync(string connectionId, string groupId)
        {
            return hub.Groups.RemoveFromGroupAsync(connectionId, groupId);
        }
        #endregion
    }
}