using Domain.DomainException;
using Domain.Other.IdentityDomain.Enum;
using Domain.Shared;

namespace Domain.Other.IdentityDomain
{
    public class User
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; private set; }
        public Role Role { get; private set; }
        public string Name { get; private set; }
        public string PreferredLocale { get; private set; }
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

        protected User() 
        { 
        
        }

        public User(
            string id, 
            string name,
            string preferredLocale,
            Role role,
            Password? password = null,
            string? email = null,
            string? steamId = null)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new BadRequest(ResponseCode.User_InvalidId);

            if (string.IsNullOrWhiteSpace(name))
                throw new BadRequest(ResponseCode.User_InvalidName);

            if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(steamId))
                throw new BadRequest(ResponseCode.User_MissingAuth);

            ID = id;
            Role = role;
            Name = name;
            PreferredLocale = preferredLocale;
            Email = email;
            PasswordHash = password?.Hash;
            SteamID = steamId;
            CreatedAt = DateTime.UtcNow;
            LastLogin = DateTime.UtcNow;
        }

        #region Methods
        public void VerifyPassword(string plainPassword)
        {
            if (PasswordHash == null)
                throw new Unauthorized(ResponseCode.User_PasswordNotSet);

            var password = Password.FromHash(PasswordHash);

            if (!password.Verify(plainPassword))
                throw new Unauthorized(ResponseCode.User_InvalidCredentials);
        }

        public void UpdateLastLogin()
        {
            LastLogin = DateTime.UtcNow;
        }

        public void SetRefreshToken(string token, DateTime expiry)
        {
            RefreshToken = token;
            RefreshTokenExpiry = expiry;
        }

        public void ValidateRefreshToken(string token, DateTime now)
        {
            if (RefreshToken != token)
                throw new Unauthorized(ResponseCode.User_InvalidRefreshToken);

            if (!RefreshTokenExpiry.HasValue || RefreshTokenExpiry <= now)
                throw new Unauthorized(ResponseCode.User_ExpiredRefreshToken);
        }

        public void UpdateProfile(
            string? name,
            DateTime? dob,
            string? gender)
        {
            if (name != null)
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new BadRequest(ResponseCode.User_InvalidName);

                Name = name;
            }

            if (dob.HasValue)
            {
                if (dob.Value == default)
                    throw new BadRequest(ResponseCode.User_InvalidDob);

                Dob = dob.Value;
            }

            if (gender != null)
            {
                if (string.IsNullOrWhiteSpace(gender))
                    throw new BadRequest(ResponseCode.User_InvalidGender);

                Gender = gender;
            }
        }
        #endregion
    }
}