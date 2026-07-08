using Contract.Enum.IdentityDomain;
using Domain.DomainException;
using ResponseCode;

namespace Domain.Definition.IdentityDomain
{
    public class User
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; private set; } = string.Empty;
        public Role Role { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public DateTime? Dob { get; private set; }
        public string? Gender { get; private set; }
        public string? Email { get; private set; }
        public string? PasswordHash { get; private set; }
        public string? SteamID { get; private set; }
        public string? RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiry { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime LastLogin { get; private set; }
        #endregion

        protected User() { }

        public User(
            string id, 
            string name,
            Role role,
            Password? password = null,
            string? email = null,
            string? steamId = null)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new BadRequest(
                    DomainCode.UserCode.InvalidId,
                    "User registration failed. ID cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(name))
                throw new BadRequest(
                    DomainCode.UserCode.InvalidName,
                    $"User registration failed for ID: '{id}'. Name cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(steamId))
                throw new BadRequest(
                    DomainCode.UserCode.MissingAuth,
                    $"User registration failed for ID: '{id}', Name: '{name}'. Requires either an Email or Steam ID.");

            ID = id;
            Role = role;
            Name = name;
            Email = email;
            PasswordHash = password?.Hash;
            SteamID = steamId;
            CreatedAt = DateTime.UtcNow;
            LastLogin = DateTime.UtcNow;
        }

        #region Methods
        public void VerifyPassword(
            string plainPassword)
        {
            if (PasswordHash == null)
                throw new Unauthorized(
                    DomainCode.UserCode.PasswordNotSet,
                    $"Authentication failed. No password hash is associated with User ID: '{ID}' ({Name}).");

            var password = Password.FromHash(PasswordHash);

            if (!password.Verify(plainPassword))
                throw new Unauthorized(
                    DomainCode.UserCode.InvalidCredentials,
                    $"Authentication failed. Invalid credentials provided for User ID: '{ID}' ({Name}).");
        }

        public void UpdateLastLogin()
        {
            LastLogin = DateTime.UtcNow;
        }

        public void SetRefreshToken(
            string token, 
            DateTime expiry)
        {
            RefreshToken = token;
            RefreshTokenExpiry = expiry;
        }

        public void ValidateRefreshToken(
            string token,
            DateTime now)
        {
            if (RefreshToken != token)
                throw new Unauthorized(
                    DomainCode.UserCode.InvalidRefreshToken,
                    $"Session validation failed. The provided refresh token is invalid for User ID: '{ID}' ({Name}).");

            if (!RefreshTokenExpiry.HasValue || RefreshTokenExpiry <= now)
                throw new Unauthorized(
                    DomainCode.UserCode.ExpiredRefreshToken,
                    $"Session validation failed. The refresh token has expired for User ID: '{ID}' ({Name}).");
        }

        public void UpdateProfile(
            string? name,
            DateTime? dob,
            string? gender)
        {
            if (name != null)
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new BadRequest(
                        DomainCode.UserCode.InvalidName,
                        $"Profile update failed for User ID: '{ID}' ({Name}). Updated name cannot be empty.");

                Name = name;
            }

            if (dob.HasValue)
            {
                if (dob.Value == default)
                    throw new BadRequest(
                        DomainCode.UserCode.InvalidDob,
                        $"Profile update failed for User ID: '{ID}' ({Name}). Invalid date of birth provided.");

                Dob = dob.Value;
            }

            if (gender != null)
            {
                if (string.IsNullOrWhiteSpace(gender))
                    throw new BadRequest(
                        DomainCode.UserCode.InvalidGender,
                        $"Profile update failed for User ID: '{ID}' ({Name}). Updated gender cannot be empty.");

                Gender = gender;
            }
        }
        #endregion
    }

    public class Password
    {
        #region Attributes
        #endregion

        #region Properties
        public string Hash { get; private set; } = string.Empty;
        #endregion

        private Password() { }

        #region Methods
        public static Password Create(string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(plainPassword))
                throw new BadRequest(
                    DomainCode.PasswordCode.PasswordRequired,
                    "Hashing execution halted. Plaintext password value is missing or empty.");

            var hash = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            return new Password
            {
                Hash = hash
            };
        }

        public static Password FromHash(string hash)
        {
            return new Password
            {
                Hash = hash
            };
        }

        public bool Verify(string plainPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, Hash);
        }
        #endregion
    }
}