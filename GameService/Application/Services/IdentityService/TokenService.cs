using Application.Interfaces.Security;
using Domain.Other.IdentityDomain;

namespace Application.Services.IdentityService
{
    public class TokenService
    {
        #region Attributes
        private readonly ITokenGenerator tokenGenerator;
        #endregion

        #region Properties
        #endregion

        public TokenService(
            ITokenGenerator tokenGenerator)
        {
            this.tokenGenerator = tokenGenerator;
        }

        #region Methods
        public (string access, string refresh) Generate(
            User user)
        {
            var access = tokenGenerator.GenerateAccessToken(user.ID, user.SteamID ?? "", user.Role.ToString());
            var refresh = tokenGenerator.GenerateRefreshToken();
            user.SetRefreshToken(refresh, tokenGenerator.GetRefreshTokenExpiry());

            return (access, refresh);
        }
        #endregion
    }
}
