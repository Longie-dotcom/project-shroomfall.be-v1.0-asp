using Domain.Abstraction;

namespace Application.Interfaces.Repository.Base
{
    public interface ISQLGenericRepository<T> 
        where T : class
    {
        Task<T?> GetByIdAsync<TKey>(
            TKey id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(
            T entity);
        Task UpdateAsync(
            T entity);
        Task DeleteAsync<TKey>(
            TKey id);
    }

    public interface ISQLDefinitionRepository<T> : ISQLGenericRepository<T>
        where T : ComponentDefinition
    {
        Task<T?> GetByEntityIdAsync(string entityDefinitionId);
        Task UpsertAsync(T entity);
    }
}