using Domain.Abstraction;

namespace Application.Interface.Repository.Base
{
    public interface IGenericRepository<T>
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
