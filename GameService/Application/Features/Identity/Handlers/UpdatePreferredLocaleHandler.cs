using Application.Features.Abstraction;
using Application.Features.Identity.Commands;
using Application.Interfaces.Cache;
using Application.Interfaces.Repository.Relational;
using Domain.DomainException;
using Domain.Shared;

namespace Application.Features.Identity.Handlers
{
    public class UpdatePreferredLocaleHandler : IHandler<UpdatePreferredLocaleCommand>
    {
        #region Attributes
        private readonly IRelationalUoW relational;
        private readonly ILocaleCache localeCache;
        #endregion

        #region Properties
        #endregion

        public UpdatePreferredLocaleHandler(
            IRelationalUoW relational,
            ILocaleCache localeCache)
        {
            this.relational = relational;
            this.localeCache = localeCache;
        }

        #region Methods
        public async Task Handle(
            UpdatePreferredLocaleCommand command)
        {
            // Validate locale existence
            if (!localeCache.Exists(command.PreferredLocale))
                throw new BadRequest(
                    ResponseCode.UpdateProfile_LocaleFound,
                    $"Locale: {command.PreferredLocale} is not existed.");

            // Resolve repository
            var userRepo = relational.GetRepository<IUserRepository>();

            // Validate existence
            var user = await userRepo.GetByIdAsync(command.UserID);
            if (user == null)
                throw new NotFound(
                    ResponseCode.UpdateProfile_UserNotFound,
                    $"User with user ID: {command.UserID} was not found");

            // Apply domain - Update preferred locale
            user.UpdatePreferredLocale(command.PreferredLocale);

            // Apply persistence
            await relational.SaveChangesAsync();
        }
        #endregion
    }
}