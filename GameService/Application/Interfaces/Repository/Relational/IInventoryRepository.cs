using Domain.Definition.ItemDomain;

namespace Application.Interfaces.Repository.Relational
{
    public interface IInventoryRepository : ISQLGenericRepository<Inventory>, IRelationalRepository
    {
        Task<List<Inventory>> GetAllWithDefaultItemsAsync();
    }
}
