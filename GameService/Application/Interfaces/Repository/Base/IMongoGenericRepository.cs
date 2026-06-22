using Domain.Abstraction;

namespace Application.Interfaces.Repository.Base
{
    public interface IMongoGenericRepository<T>
        where T : class, ISnapshot
    {
        Task<T?> GetByIdAsync(
            string id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(
            T entity);
        Task UpdateAsync(
            T entity);
        Task UpdateManyAsync(
            IEnumerable<T> entities);
        Task DeleteAsync(
            string id);
    }
}
