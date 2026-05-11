namespace Application.DTO.Identity
{
    public class SteamAuthDTO
    {
        public string SteamTicket { get; set; } = string.Empty;
        public string? SteamName { get; set; }
        public string PreferredLocale { get; set; } = string.Empty;
    }

    public class RegisterDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string PreferredLocale { get; set; } = string.Empty;
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
        public string? Name { get; set; } = string.Empty;
        public string? Gender { get; set; } = string.Empty;
        public DateTime? Dob { get; set; }
    }
}