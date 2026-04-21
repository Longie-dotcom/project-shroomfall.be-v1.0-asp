namespace Domain.IdentityDomain
{
    public class Password
    {
        #region Properties
        public string Hash { get; private set; }
        #endregion

        private Password() { }

        #region Factory Methods
        public static Password Create(string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(plainPassword))
                throw new ArgumentException("Password cannot be empty.");

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