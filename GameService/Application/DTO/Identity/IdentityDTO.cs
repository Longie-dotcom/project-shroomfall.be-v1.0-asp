using Domain.Other.IdentityDomain.Enum;

namespace Application.DTO.Identity
{
    public class UserDTO
    {
        public string ID { get; set; } = string.Empty;
        public Role Role { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PreferredLocale { get; set; } = string.Empty;
        public DateTime? Dob { get; set; }
        public string? Gender { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? SteamID { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastLogin { get; set; }
    }

    public class TokenDTO
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}