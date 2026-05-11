using Application.DTO.Identity;
using Application.Features.Abstraction;
using Application.Features.Identity.Commands;
using Application.Interfaces.Repository.Relational;
using Application.Services.Abstraction.OtherService;
using Domain.DomainException;
using Domain.Shared;

namespace Application.Features.Identity.Handlers
{
    public class RefreshTokenHandler : IHandler<RefreshTokenCommand, TokenDTO>
    {
        #region Attributes
        private readonly IRelationalUoW relational;
        private readonly ITokenService tokenService;
        #endregion

        #region Properties
        #endregion

        public RefreshTokenHandler(
            IRelationalUoW relational,
            ITokenService tokenService)
        {
            this.relational = relational;
            this.tokenService = tokenService;
        }

        #region Methods
        public async Task<TokenDTO> Handle(
            RefreshTokenCommand command)
        {
            var dto = command.DTO;

            // Resolve repository
            var userRepo = relational.GetRepository<IUserRepository>();

            // Validate authentication
            var user = await userRepo.GetByIdAsync(command.UserID);
            if (user == null)
                throw new NotFound(ResponseCode.RefreshToken_UserNotFound);

            // Validate refresh token 
            user.ValidateRefreshToken(dto.RefreshToken, DateTime.UtcNow);

            // Apply domain - Set token
            (var accessToken, var newRefreshToken) = tokenService.Generate(user);

            // Apply persistence
            await relational.SaveChangesAsync();

            return new TokenDTO
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken
            };
        }
        #endregion
    }
}