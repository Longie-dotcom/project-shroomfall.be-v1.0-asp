using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Security
{
    public class TokenGenerator
    {
        #region Attributes
        private readonly string jwtKey;
        private readonly string issuer;
        private readonly string audience;
        #endregion

        #region Properties
        #endregion

        public TokenGenerator(
            string jwtKey,
            string issuer,
            string audience)
        {
            this.jwtKey = jwtKey;
            this.issuer = issuer;
            this.audience = audience;
        }

        #region Methods
        public string GenerateAccessToken(string userId, string steamId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim("steamId", steamId ?? "")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            return Convert.ToBase64String(randomBytes);
        }

        public DateTime GetRefreshTokenExpiry()
        {
            return DateTime.UtcNow.AddDays(7);
        }
        #endregion
    }
}