using Application.DTO.Identity;
using Application.Features.Abstraction;
using Application.Features.Identity.Commands;
using Application.Interfaces.Repository.Relational;
using Application.Services.Abstraction.OtherService;
using Domain.DomainException;
using Domain.Other.IdentityDomain;
using Domain.Other.IdentityDomain.Enum;
using Domain.Shared;

namespace Application.Features.Identity.Handlers
{
    public class RegisterHandler : IHandler<RegisterCommand, TokenDTO>
    {
        #region Attributes
        private readonly IRelationalUoW relational;
        private readonly ITokenService tokenService;
        #endregion

        #region Properties
        #endregion

        public RegisterHandler(
            IRelationalUoW relational,
            ITokenService tokenService)
        {
            this.relational = relational;
            this.tokenService = tokenService;
        }

        #region Methods
        public async Task<TokenDTO> Handle(
            RegisterCommand command)
        {
            var dto = command.DTO;

            // Resolve repositories
            var userRepo = relational.GetRepository<IUserRepository>();

            // Validate fields
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new BadRequest(ResponseCode.Register_EmailRequired);

            // Validate email existence
            var email = dto.Email.Trim().ToLowerInvariant();
            if (await userRepo.EmailExistsAsync(email))
                throw new BadRequest(ResponseCode.Register_EmailAlreadyExists);

            // Apply domain - Create user
            var user = new User(
                id: Guid.NewGuid().ToString(),
                name: dto.Name ?? "Player",
                preferredLocale: dto.PreferredLocale,
                role: Role.Player,
                password: Password.Create(dto.Password),
                email: email
            );

            // Apply domain - Login and set token
            user.UpdateLastLogin();
            (var accessToken, var refreshToken) = tokenService.Generate(user);

            // Apply persistence
            await relational.BeginTransactionAsync();
            await userRepo.AddAsync(user);
            await relational.CommitAsync();

            return new TokenDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        #endregion
    }
}