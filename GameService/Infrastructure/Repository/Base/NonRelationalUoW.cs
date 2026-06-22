using Application.Interfaces.Repository.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Repository.Base
{
    public class NonRelationalUoW : INonRelationalUoW
    {
        #region Attributes
        private readonly IServiceProvider provider;
        private readonly Dictionary<Type, object> repositories = new();
        #endregion

        #region Properties
        #endregion

        public NonRelationalUoW(
            IServiceProvider provider)
        {
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