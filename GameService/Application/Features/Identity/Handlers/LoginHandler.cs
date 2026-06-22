using Application.Features.Abstraction;
using Application.Features.Identity.Commands;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using Application.Services.IdentityService;
using Contract.DTO.Identity;
using Domain.Shared.DomainException;
using Domain.Shared.ResponseCode;

namespace Application.Features.Identity.Handlers
{
    public class LoginHandler : IHandler<LoginCommand, TokenDTO>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        private readonly TokenService tokenService;
        #endregion

        #region Properties
        #endregion

        public LoginHandler(
            IRelationalUoW relationalUoW,
            TokenService tokenService)
        {
            this.relationalUoW = relationalUoW;
            this.tokenService = tokenService;
        }

        #region Methods
        public async Task<TokenDTO> Handle(
            LoginCommand command)
        {
            var dto = command.DTO;

            // Resolve repository
            var userRepo = relationalUoW.GetRepository<IUserRepository>();

            // Validate input
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new BadRequest(
                    ApplicationCode.IdentityHandlerCode.LoginEmailRequired,
                    $"Email is required in login, login process was terminated");

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new BadRequest(
                    ApplicationCode.IdentityHandlerCode.LoginPasswordRequired,
                    $"Password is required in login, login process was terminated");

            // Validate authentication
            var email = dto.Email.Trim().ToLowerInvariant();
            var user = await userRepo.GetByEmailAsync(email);
            if (user == null)
                throw new Unauthorized(
                    ApplicationCode.IdentityHandlerCode.LoginInvalidCredentials,
                    $"Credential is invalid");
            user.VerifyPassword(dto.Password);

            // Apply domain - Login and set token
            user.UpdateLastLogin();
            (var accessToken, var refreshToken) = tokenService.Generate(user);

            // Apply persistence
            await relationalUoW.SaveChangesAsync();

            return new TokenDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        #endregion
    }
}