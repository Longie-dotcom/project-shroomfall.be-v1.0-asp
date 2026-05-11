namespace Application.Interfaces.Repository.Relational
{
    public interface ISQLGenericRepository<T> 
        where T : class
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