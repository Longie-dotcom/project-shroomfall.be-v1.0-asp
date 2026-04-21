using Infrastructure.Persistence;
using Infrastructure.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        #region Attributes
        private readonly DbSet<T> dbSet;
        #endregion

        #region Properties
        #endregion

        public GenericRepository(RelationalDB context)
        {
            this.dbSet = context.Set<T>();
        }

        #region Methods
        public async Task<T?> GetByIdAsync(string id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            dbSet.Update(entity);
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return;

            dbSet.Remove(entity);
        }
        #endregion
    }
}