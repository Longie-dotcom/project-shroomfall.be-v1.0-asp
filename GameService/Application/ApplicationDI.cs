using Application.Helper;
using Application.Service.Implementation;
using Application.Service.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class ApplicationDI
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        #region Methods
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => {
                cfg.AddProfile<Mapper>();
            });

            services.AddScoped<IIdentityService, IdentityService>();

            return services;
        }
        #endregion
    }
}