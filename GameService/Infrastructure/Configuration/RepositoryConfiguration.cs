using Application.Interface.Repository;
using Application.Interface.Repository.Base;
using Infrastructure.Repository;
using Infrastructure.Repository.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Configuration
{
    public static class RepositoryConfiguration
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        #region Methods
        public static IServiceCollection AddRepositoryConfiguration(
            this IServiceCollection services)
        {
            // UNIT OF WORK
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // REPOSITORY
            services.AddScoped<IEntitySnapshotRepository, EntitySnapshotRepository>();
            services.AddScoped<IRoomSnapshotRepository, RoomSnapshotRepository>();

            return services;
        }
        #endregion
    }
}