using Domain.Abstraction;

namespace Application.Interfaces.Repository.NonRelational
{
    public interface IMongoGenericRepository<T>
        where T : class, IDocumentObject
    {
        Task<T?> GetByIdAsync(
            string id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(
            T entity);
        Task UpdateAsync(
            T entity);
        Task DeleteAsync(
            string id);
    }
}
