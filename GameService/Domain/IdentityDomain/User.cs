using Domain.DomainException;
using Domain.Shared;

namespace Domain.IdentityDomain
{
    public class User
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; private set; }
        public string PlayerID { get; private set; }
        public string Name { get; private set; }
        public DateTime? Dob { get; private set; }
        public string? Gender { get; private set; }

        public string? Email { get; private set; }
        public string? PasswordHash { get; private set; }
        public string? SteamID { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime LastLogin { get; private set; }

        public byte[] RowVersion { get; private set; }

        public string? RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiry { get; private set; }
        #endregion

        protected User() 
        { 
        
        }

        public User(
            string id, 
            string playerId,
            string name,
            string? email = null, 
            string? passwordHash = null, 
            string? steamId = null)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new BadRequest(FailedCode.Invalid_Id);

            if (string.IsNullOrWhiteSpace(playerId))
                throw new BadRequest(FailedCode.User_InvalidPlayerId);

            if (string.IsNullOrWhiteSpace(name))
                throw new BadRequest(FailedCode.User_InvalidName);

            if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(steamId))
                throw new BadRequest(FailedCode.User_MissingAuth);

            ID = id;
            PlayerID = playerId;

            Name = name;
            Email = email;
            PasswordHash = passwordHash;
            SteamID = steamId;

            CreatedAt = DateTime.UtcNow;
            LastLogin = DateTime.UtcNow;
        }

        #region Methods
        public void SetPassword(Password password)
        {
            PasswordHash = password.Hash;
        }

        public void VerifyPassword(string plainPassword)
        {
            if (PasswordHash == null)
                throw new Unauthorized(FailedCode.User_PasswordNotSet);

            var password = Password.FromHash(PasswordHash);

            if (!password.Verify(plainPassword))
                throw new Unauthorized(FailedCode.User_InvalidCredentials);
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

        public bool IsRefreshTokenValid(string token)
        {
            return RefreshToken == token &&
                   RefreshTokenExpiry.HasValue &&
                   RefreshTokenExpiry > DateTime.UtcNow;
        }

        public void UpdateProfile(
            string? name,
            DateTime? dob,
            string? gender)
        {
            if (name != null)
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new BadRequest(FailedCode.User_InvalidName);

                Name = name;
            }

            if (dob.HasValue)
            {
                if (dob.Value == default)
                    throw new BadRequest(FailedCode.User_InvalidDob);

                Dob = dob.Value;
            }

            if (gender != null)
            {
                if (string.IsNullOrWhiteSpace(gender))
                    throw new BadRequest(FailedCode.User_InvalidGender);

                Gender = gender;
            }
        }
        #endregion
    }
}