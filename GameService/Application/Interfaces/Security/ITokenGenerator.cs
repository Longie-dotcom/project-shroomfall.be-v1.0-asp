namespace Application.Interfaces.Security
{
    public interface ITokenGenerator
    {
        string GenerateAccessToken(
            string userId, 
            string steamId,
            string role);
        string GenerateRefreshToken();
        DateTime GetRefreshTokenExpiry();
    }
}
