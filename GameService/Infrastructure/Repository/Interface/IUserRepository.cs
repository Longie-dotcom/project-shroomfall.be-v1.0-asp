using Domain.IdentityDomain;

namespace Infrastructure.Repository.Interface
{
    public interface IUserRepository : IRelationalRepository, IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetBySteamIdAsync(string steamId);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> SteamExistsAsync(string steamId);
    }
}
