using Domain.DomainException;
using Domain.Shared;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Helper
{
    public static class HubContextHelper
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        #region Methods
        public static (string UserId, string ConnectionId) GetValidatedContext(
            Hub hub)
        {
            var userId = hub.Context.UserIdentifier;
            var connectionId = hub.Context.ConnectionId;

            if (string.IsNullOrWhiteSpace(userId))
                throw new Unauthorized(
                    ResponseCode.HubContextHelper_UserIdNotFound,
                    "A user ID is missing from SignalR context.");

            if (string.IsNullOrWhiteSpace(connectionId))
                throw new Unauthorized(
                    ResponseCode.HubContextHelper_ConnectionIdNotFound,
                    "A connection ID is missing from SignalR context.");

            return (userId, connectionId);
        }
        #endregion
    }
}