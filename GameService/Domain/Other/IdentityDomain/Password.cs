using Domain.DomainException;
using Domain.Shared;

namespace Domain.Other.IdentityDomain
{
    public class Password
    {
        #region Attributes
        #endregion

        #region Properties
        public string Hash { get; private set; }
        #endregion

        private Password()
        { 
        
        }

        #region Methods
        public static Password Create(string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(plainPassword))
                throw new BadRequest(ResponseCode.User_PasswordRequired);

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