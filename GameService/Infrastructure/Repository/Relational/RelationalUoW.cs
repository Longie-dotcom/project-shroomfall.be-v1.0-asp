using Application.Interfaces.Repository.Relational;
using Domain.DomainException;
using Domain.Shared;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Repository.Relational
{
    public class RelationalUoW : IRelationalUoW
    {
        #region Attributes
        private readonly RelationalDB context;
        private readonly IServiceProvider provider;
        private readonly Dictionary<Type, object> repositories = new();

        private IDbContextTransaction? transaction;
        #endregion

        #region Properties
        #endregion

        public RelationalUoW(
            RelationalDB context, 
            IServiceProvider provider)
        {
            this.context = context;
            this.provider = provider;
        }

        #region Methods
        public T GetRepository<T>() where T : IRelationalRepository
        {
            var type = typeof(T);

            if (!repositories.TryGetValue(type, out var repo))
            {
                repo = provider.GetRequiredService<T>();
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
                throw new InternalException(ResponseCode.RelationalUoW_NoTransactionCreated);

            try
            {
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return 1;
            }
            catch
            {
                await RollbackAsync();
                throw new InternalException(ResponseCode.RelationalUoW_CommitException);
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
