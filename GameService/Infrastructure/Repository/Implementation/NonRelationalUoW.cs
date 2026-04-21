using Infrastructure.Persistence;
using Infrastructure.Repository.Interface;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Infrastructure.Repository.Implementation
{
    public class NonRelationalUoW
    {
        #region Attributes
        private readonly NonRelationalDB context;
        private readonly IServiceProvider provider;
        private readonly Dictionary<Type, object> repositories = new();

        private IClientSessionHandle? session;
        #endregion

        #region Properties
        public IClientSessionHandle? Session
        {
            get { return session; }
        }
        #endregion

        public NonRelationalUoW(
            NonRelationalDB context,
            IServiceProvider provider)
        {
            this.context = context;
            this.provider = provider;
        }

        #region Methods
        public T GetRepository<T>() where T : INonRelationalRepository
        {
            var type = typeof(T);

            if (!repositories.TryGetValue(type, out var repo))
            {
                repo = provider.GetRequiredService(type);
                repositories[type] = repo;
            }

            return (T)repo;
        }
        #endregion
    }
}