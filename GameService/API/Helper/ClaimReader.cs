using System.Security.Claims;

namespace API.Helper
{
    public static class ClaimReader
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        #region Methods
        public static (string UserId, string? SteamId) GetIdentity(ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User ID claim missing");

            var steamId = user.FindFirst("steamId")?.Value;

            return (userId, steamId);
        }

        public static bool IsAuthenticated(ClaimsPrincipal user)
        {
            return user?.Identity?.IsAuthenticated ?? false;
        }
        #endregion
    }
}
