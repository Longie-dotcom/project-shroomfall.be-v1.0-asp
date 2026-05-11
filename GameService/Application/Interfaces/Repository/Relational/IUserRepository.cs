using Domain.Other.IdentityDomain;

namespace Application.Interfaces.Repository.Relational
{
    public interface IUserRepository : ISQLGenericRepository<User>, IRelationalRepository
    {
        Task<User?> GetByEmailAsync(
            string email);
        Task<User?> GetBySteamIdAsync(
            string steamId);
        Task<bool> EmailExistsAsync(
            string email);
        Task<bool> SteamExistsAsync(
            string steamId);
    }
}
