using Domain.IdentityDomain;
using Infrastructure.Persistence;
using Infrastructure.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Implementation
{
    public class UserRepository : GenericRepository<User>, IUserRepository, IRelationalRepository
    {
        #region Attributes
        private readonly RelationalDB context;
        #endregion

        #region Properties
        #endregion

        public UserRepository(RelationalDB context) : base(context)
        {
            this.context = context;
        }

        #region Methods
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await context.Users
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User?> GetBySteamIdAsync(string steamId)
        {
            return await context.Users
                .FirstOrDefaultAsync(x => x.SteamID == steamId);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await context.Users
                .AnyAsync(x => x.Email == email);
        }

        public async Task<bool> SteamExistsAsync(string steamId)
        {
            return await context.Users
                .AnyAsync(x => x.SteamID == steamId);
        }
        #endregion
    }
}
