namespace Application.DTO
{
    public class SteamAuthDTO
    {
        public string SteamTicket { get; set; } = string.Empty;
        public string? SteamName { get; set; }
    }

    public class RegisterDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Name { get; set; } = string.Empty;
        public string? Gender { get; set; } = string.Empty;
        public DateTime? Dob { get; set; }
    }

    public class LoginDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Password { set; get; } = string.Empty;
    }

    public class RefreshTokenDTO
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class UpdateProfileDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime Dob { get; set; }
    }

    public class UserDTO
    {
        public string ID { get; set; } = string.Empty;
        public string PlayerID { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime? Dob { get; set; }
        public string? Gender { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? SteamID { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastLogin { get; set; }
        public byte[] RowVersion { get; set; } = new byte[4];
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
    }

    public class TokenDTO
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}