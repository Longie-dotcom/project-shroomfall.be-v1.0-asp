using Application.Features.Abstraction;
using Application.Features.Identity.Commands;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;

namespace Application.Features.Identity.Handlers
{
    public class UpdateProfileHandler : IHandler<UpdateProfileCommand>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        #endregion

        #region Properties
        #endregion

        public UpdateProfileHandler(
            IRelationalUoW relationalUoW)
        {
            this.relationalUoW = relationalUoW;
        }

        #region Methods
        public async Task Handle(
            UpdateProfileCommand command)
        {
            var dto = command.DTO;

            // Resolve repository
            var userRepo = relationalUoW.GetRepository<IUserRepository>();

            // Validate existence
            var user = await userRepo.GetByIdAsync(command.UserID);
            if (user == null)
                throw new NotFound(
                    ApplicationCode.IdentityHandlerCode.UpdateProfileUserNotFound,
                    $"User with user ID: {command.UserID} was not found");

            // Apply domain - Update profile
            user.UpdateProfile(
                dto.Name,
                dto.Dob,
                dto.Gender
            );

            // Apply persistence
            await relationalUoW.SaveChangesAsync();
        }
        #endregion
    }
}