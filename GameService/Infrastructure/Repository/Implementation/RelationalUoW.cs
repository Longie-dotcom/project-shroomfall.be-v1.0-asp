using Infrastructure.Persistence;
using Infrastructure.Repository.Interface;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Repository.Implementation
{
    public class RelationalUoW : IRelationalUoW
    {
        #region Attributes
        private readonly RelationalDB context;
        private readonly IServiceProvider serviceProvider;
        private readonly Dictionary<Type, object> repositories = new();

        private IDbContextTransaction? transaction;
        #endregion

        #region Properties
        #endregion

        public RelationalUoW(
            RelationalDB context, 
            IServiceProvider serviceProvider)
        {
            this.context = context;
            this.serviceProvider = serviceProvider;
        }

        #region Methods
        public T GetRepository<T>() where T : IRelationalRepository
        {
            var type = typeof(T);

            if (!repositories.TryGetValue(type, out var repo))
            {
                repo = serviceProvider.GetRequiredService<T>();
                repositories[type] = repo!;
            }

            return (T)repo!;
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

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
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
