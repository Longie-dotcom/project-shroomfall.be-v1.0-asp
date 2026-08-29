using Application.Mapper;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Configuration
{
    public static class MapperConfiguration
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        #region Methods
        public static IServiceCollection AddMapperConfiguration(
            this IServiceCollection services)
        {
            // DTO MAPPER
            services.AddAutoMapper(cfg => { cfg.AddProfile<DTOMapper>(); });
            
            // SNAPSHOT MAPPER
            services.AddAutoMapper(cfg => { cfg.AddProfile<SnapshotMapper>(); });

            return services;
        }
        #endregion
    }
}