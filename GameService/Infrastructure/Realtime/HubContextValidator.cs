using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Realtime
{
    public static class HubContextValidator
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
                    InfrastructureCode.HubContextValidatorCode.UserIdNotFound,
                    "A user ID is missing from SignalR context.");

            if (string.IsNullOrWhiteSpace(connectionId))
                throw new Unauthorized(
                    InfrastructureCode.HubContextValidatorCode.ConnectionIdNotFound,
                    "A connection ID is missing from SignalR context.");

            return (userId, connectionId);
        }
        #endregion
    }
}