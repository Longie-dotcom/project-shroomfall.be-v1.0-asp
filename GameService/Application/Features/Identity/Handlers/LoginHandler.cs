using Application.DTO.Identity;
using Application.Features.Abstraction;
using Application.Features.Identity.Commands;
using Application.Interfaces.Repository.Relational;
using Application.Services.IdentityService;
using Domain.DomainException;
using Domain.Shared;

namespace Application.Features.Identity.Handlers
{
    public class LoginHandler : IHandler<LoginCommand, TokenDTO>
    {
        #region Attributes
        private readonly IRelationalUoW relational;
        private readonly TokenService tokenService;
        #endregion

        #region Properties
        #endregion

        public LoginHandler(
            IRelationalUoW relational,
            TokenService tokenService)
        {
            this.relational = relational;
            this.tokenService = tokenService;
        }

        #region Methods
        public async Task<TokenDTO> Handle(
            LoginCommand command)
        {
            var dto = command.DTO;

            // Resolve repository
            var userRepo = relational.GetRepository<IUserRepository>();

            // Validate input
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new BadRequest(
                    ResponseCode.Login_EmailRequired,
                    $"Email is required in login, login process was terminated");

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new BadRequest(
                    ResponseCode.Login_PasswordRequired,
                    $"Password is required in login, login process was terminated");

            // Validate authentication
            var email = dto.Email.Trim().ToLowerInvariant();
            var user = await userRepo.GetByEmailAsync(email);
            if (user == null)
                throw new Unauthorized(
                    ResponseCode.Login_InvalidCredentials, 
                    $"Credential is invalid");
            user.VerifyPassword(dto.Password);

            // Apply domain - Login and set token
            user.UpdateLastLogin();
            (var accessToken, var refreshToken) = tokenService.Generate(user);

            // Apply persistence
            await relational.SaveChangesAsync();

            return new TokenDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        #endregion
    }
}