using Application.Interfaces.Repository.Relational;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Relational
{
    public class SQLGenericRepository<T> : ISQLGenericRepository<T> where T : class
    {
        #region Attributes
        private readonly DbSet<T> dbSet;
        #endregion

        #region Properties
        #endregion

        public SQLGenericRepository(
            RelationalDB context)
        {
            this.dbSet = context.Set<T>();
        }

        #region Methods
        public async Task<T?> GetByIdAsync(
            string id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
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

        public async Task DeleteAsync(
            string id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return;

            dbSet.Remove(entity);
        }
        #endregion
    }
}