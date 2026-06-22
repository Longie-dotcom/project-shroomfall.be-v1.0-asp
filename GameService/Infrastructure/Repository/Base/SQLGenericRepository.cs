using Application.Interfaces.Repository.Base;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Base
{
    public class SQLGenericRepository<T> : ISQLGenericRepository<T> where T : class
    {
        #region Attributes
        protected readonly DbSet<T> dbSet;
        protected readonly RelationalDB context;
        #endregion

        #region Properties
        #endregion

        public SQLGenericRepository(
            RelationalDB context)
        {
            dbSet = context.Set<T>();
            this.context = context;
        }

        #region Methods
        public async Task<T?> GetByIdAsync<TKey>(
            TKey id)
        {
            return await dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await dbSet.AsNoTracking().ToListAsync();
        }

        public async Task AddAsync(
            T entity)
        {
            await dbSet.AddAsync(entity);
        }

        public async Task UpdateAsync(
            T entity)
        {
            dbSet.Update(entity);
        }

        public async Task DeleteAsync<TKey>(
            TKey id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return;

            dbSet.Remove(entity);
        }
        #endregion
    }
}