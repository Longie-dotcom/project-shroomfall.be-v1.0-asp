using Application.Features.Abstraction;
using Application.Features.Identity.Commands;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using Application.Services.IdentityService;
using Contract.DTO.Feature.Identity.Response;
using Domain.DomainException;
using ResponseCode;

namespace Application.Features.Identity.Handlers
{
    public class RefreshTokenHandler : IHandler<RefreshTokenCommand, TokenDTO>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        private readonly TokenService tokenService;
        #endregion

        #region Properties
        #endregion

        public RefreshTokenHandler(
            IRelationalUoW relationalUoW,
            TokenService tokenService)
        {
            this.relationalUoW = relationalUoW;
            this.tokenService = tokenService;
        }

        #region Methods
        public async Task<TokenDTO> Handle(
            RefreshTokenCommand command)
        {
            var dto = command.DTO;

            // Resolve repository
            var userRepo = relationalUoW.GetRepository<IUserRepository>();

            // Validate authentication
            var user = await userRepo.GetByIdAsync(command.UserID);
            if (user == null)
                throw new NotFound(
                    ApplicationCode.IdentityHandlerCode.RefreshTokenUserNotFound,
                    $"User with user ID: {command.UserID} was not found");

            // Validate refresh token 
            user.ValidateRefreshToken(dto.RefreshToken, DateTime.UtcNow);

            // Apply domain - Set token
            (var accessToken, var newRefreshToken) = tokenService.Generate(user);

            // Apply persistence
            await relationalUoW.SaveChangesAsync();

            return new TokenDTO
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken
            };
        }
        #endregion
    }
}