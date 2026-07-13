using Application.Features.Abstraction;
using Application.Features.Identity.Commands;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using Application.Services.IdentityService;
using Contract.DTO.Feature.Identity.Response;
using Contract.Enum.IdentityDomain;
using Domain.Definition.IdentityDomain;
using Domain.DomainException;
using ResponseCode;

namespace Application.Features.Identity.Handlers
{
    public class RegisterHandler : IHandler<RegisterCommand, TokenDTO>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        private readonly TokenService tokenService;
        #endregion

        #region Properties
        #endregion

        public RegisterHandler(
            IRelationalUoW relationalUoW,
            TokenService tokenService)
        {
            this.relationalUoW = relationalUoW;
            this.tokenService = tokenService;
        }

        #region Methods
        public async Task<TokenDTO> Handle(
            RegisterCommand command)
        {
            var dto = command.DTO;

            // Resolve repositories
            var userRepo = relationalUoW.GetRepository<IUserRepository>();

            // Validate fields
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new BadRequest(
                    ApplicationCode.IdentityHandlerCode.RegisterEmailRequired,
                    $"Email is required in registration, registration process was terminated");

            // Validate email existence
            var email = dto.Email.Trim().ToLowerInvariant();
            var existed = await userRepo.GetByEmailAsync(email);
            if (existed != null)
                throw new BadRequest(
                    ApplicationCode.IdentityHandlerCode.RegisterEmailAlreadyExists,
                    $"Email already existed when registered {dto.Email}");

            // Apply domain - Create user
            var user = new User(
                id: Guid.NewGuid().ToString(),
                name: dto.Name ?? "Player",
                role: Role.Player,
                password: Password.Create(dto.Password),
                email: email
            );

            // Apply domain - Login and set token
            user.UpdateLastLogin();
            (var accessToken, var refreshToken) = tokenService.Generate(user);

            // Apply persistence
            await relationalUoW.BeginTransactionAsync();
            await userRepo.AddAsync(user);
            await relationalUoW.CommitAsync();

            return new TokenDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        #endregion
    }
}