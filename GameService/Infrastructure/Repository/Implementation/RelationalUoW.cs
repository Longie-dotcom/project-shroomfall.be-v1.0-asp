using Infrastructure.Persistence;
using Infrastructure.Repository.Interface;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Repository.Implementation
{
    public class RelationalUoW : IRelationalUoW
    {
        #region Attributes
        private readonly RelationalDB context;
        private readonly Dictionary<Type, object> repositories = new();

        private IDbContextTransaction? transaction;
        #endregion

        #region Properties
        #endregion

        public RelationalUoW(RelationalDB context)
        {
            this.context = context;
        }

        #region Methods
        public T GetRepository<T>() where T : IRelationalRepository
        {
            var type = typeof(T);

            if (!repositories.TryGetValue(type, out var repo))
            {
                repo = Activator.CreateInstance(typeof(T), context)
                    ?? throw new InvalidOperationException($"Cannot create {type.Name}");

                repositories[type] = repo;
            }

            return (T)repo;
        }

        public async Task BeginTransactionAsync()
        {
            if (transaction == null)
                transaction = await context.Database.BeginTransactionAsync();
        }

        public async Task<int> CommitAsync()
        {
            if (transaction == null)
                throw new InvalidOperationException("Transaction not started.");

            try
            {
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return 1;
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
            finally
            {
                await transaction.DisposeAsync();
                transaction = null;
            }
        }

        private async Task RollbackAsync()
        {
            if (transaction != null)
            {
                await transaction.RollbackAsync();
                await transaction.DisposeAsync();
                transaction = null;
            }
        }
        #endregion
    }
}
