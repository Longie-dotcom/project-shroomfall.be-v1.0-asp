using Domain.Definition.ItemDomain;

namespace Application.Interfaces.Repository.Relational
{
    public interface IItemRepository : ISQLGenericRepository<Item>, IRelationalRepository
    {
        Task<IEnumerable<Item>> GetAllWithEffectsAsync();
    }
}
