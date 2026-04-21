using Domain.DomainException;

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

        public bool VerifyPassword(string plainPassword)
        {
            if (PasswordHash == null)
                return false;

            var password = Password.FromHash(PasswordHash);
            return password.Verify(plainPassword);
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
                    throw new BadRequest(
                        "Invalid name");

                Name = name;
            }

            if (dob.HasValue)
            {
                if (dob.Value == default)
                    throw new BadRequest(
                        "Invalid DOB");

                Dob = dob.Value;
            }

            if (gender != null)
            {
                if (string.IsNullOrWhiteSpace(gender))
                    throw new BadRequest(
                        "Invalid gender");

                Gender = gender;
            }
        }
        #endregion
    }
}