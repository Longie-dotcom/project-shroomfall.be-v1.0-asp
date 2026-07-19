using Contract;
using Contract.Enum.IdentityDomain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Realtime
{
    [Authorize(Roles = nameof(Role.Admin))]
    public class AdminHub : Hub
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public AdminHub() { }

        #region Methods
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"[SignalR] AdminHub: Client Connected. ConnectionID: {Context.ConnectionId}");

            await Groups.AddToGroupAsync(Context.ConnectionId, Constraint.ADMIN_REALTIME_GROUP);

            Console.WriteLine($"[SignalR] AdminHub: Added {Context.ConnectionId} to group {Constraint.ADMIN_REALTIME_GROUP}");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"[SignalR] AdminHub: Client Disconnected. ConnectionID: {Context.ConnectionId}");

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, Constraint.ADMIN_REALTIME_GROUP);

            await base.OnDisconnectedAsync(exception);
        }
        #endregion
    }
}