using Application.Interfaces.Repository.Relational;
using Domain.Definition.IdentityDomain;
using Infrastructure.Persistence;
using Infrastructure.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Relational
{
    public class UserRepository : SQLGenericRepository<User>, IUserRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public UserRepository(RelationalDB context) : base(context) { }

        #region Methods
        public async Task<User?> GetByEmailAsync(
            string email)
        {
            return await dbSet
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User?> GetBySteamIdAsync(
            string steamId)
        {
            return await dbSet
                .FirstOrDefaultAsync(x => x.SteamID == steamId);
        }
        #endregion
    }
}