using Application.Features.Abstraction;
using Application.Features.Identity.Commands;
using Application.Interfaces.Cache;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;

namespace Application.Features.Identity.Handlers
{
    public class UpdatePreferredLocaleHandler : IHandler<UpdatePreferredLocaleCommand>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        private readonly ICacheProvider cacheProvider;
        #endregion

        #region Properties
        #endregion

        public UpdatePreferredLocaleHandler(
            IRelationalUoW relationalUoW,
            ICacheProvider cacheProvider)
        {
            this.relationalUoW = relationalUoW;
            this.cacheProvider = cacheProvider;
        }

        #region Methods
        public async Task Handle(
            UpdatePreferredLocaleCommand command)
        {
            // Resolve repository
            var userRepo = relationalUoW.GetRepository<IUserRepository>();

            // Validate locale existence
            if (!cacheProvider.Locale.Exists(command.PreferredLocale))
                throw new BadRequest(
                    ApplicationCode.IdentityHandlerCode.UpdatePreferredLocaleLocaleNotFound,
                    $"Locale: {command.PreferredLocale} is not existed.");

            // Validate existence
            var user = await userRepo.GetByIdAsync(command.UserID);
            if (user == null)
                throw new NotFound(
                    ApplicationCode.IdentityHandlerCode.UpdatePreferredLocaleUserNotFound,
                    $"User with user ID: {command.UserID} was not found");

            // Apply domain - Update preferred locale
            user.UpdatePreferredLocale(command.PreferredLocale);

            // Apply persistence
            await relationalUoW.SaveChangesAsync();
        }
        #endregion
    }
}