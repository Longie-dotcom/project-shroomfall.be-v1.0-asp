using Application.Interfaces.Repository.Base;
using Domain.Definition.IdentityDomain;

namespace Application.Interfaces.Repository.Relational
{
    public interface IUserRepository : ISQLGenericRepository<User>, IRelationalRepository
    {
        Task<User?> GetByEmailAsync(
            string email);
        Task<User?> GetBySteamIdAsync(
            string steamId);
    }
}
