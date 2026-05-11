using Application.Features.Abstraction;
using Application.Features.Identity.Commands;
using Application.Interfaces.Repository.Relational;
using Domain.DomainException;
using Domain.Shared;

namespace Application.Features.Identity.Handlers
{
    public class UpdateProfileHandler : IHandler<UpdateProfileCommand>
    {
        #region Attributes
        private readonly IRelationalUoW relational;
        #endregion

        #region Properties
        #endregion

        public UpdateProfileHandler(
            IRelationalUoW relational)
        {
            this.relational = relational;
        }

        #region Methods
        public async Task Handle(
            UpdateProfileCommand command)
        {
            var dto = command.DTO;

            // Resolve repository
            var userRepo = relational.GetRepository<IUserRepository>();

            // Validate existence
            var user = await userRepo.GetByIdAsync(command.UserID);
            if (user == null)
                throw new NotFound(ResponseCode.UpdateProfile_UserNotFound);

            // Apply domain - Update profile
            user.UpdateProfile(
                dto.Name,
                dto.Dob,
                dto.Gender
            );

            // Apply persistence
            await relational.SaveChangesAsync();
        }
        #endregion
    }
}