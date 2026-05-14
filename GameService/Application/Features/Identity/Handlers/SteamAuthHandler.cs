using Application.DTO.Identity;
using Application.Features.Abstraction;
using Application.Identity.Commands;
using Application.Interfaces.Repository.Relational;
using Application.Interfaces.Security;
using Application.Services.IdentityService;
using Domain.DomainException;
using Domain.Other.IdentityDomain;
using Domain.Other.IdentityDomain.Enum;
using Domain.Shared;

namespace Application.Features.Identity.Handlers
{
    public class SteamAuthHandler : IHandler<SteamAuthCommand, TokenDTO>
    {
        #region Attributes
        private readonly IRelationalUoW relational;
        private readonly ISteamValidator steamValidator;
        private readonly TokenService tokenService;
        #endregion

        #region Properties
        #endregion

        public SteamAuthHandler(
            IRelationalUoW relational,
            ISteamValidator steamValidator,
            TokenService tokenService)
        {
            this.relational = relational;
            this.steamValidator = steamValidator;
            this.tokenService = tokenService;
        }

        #region Methods
        public async Task<TokenDTO> Handle(
            SteamAuthCommand command)
        {
            var dto = command.DTO;

            // Resolve repository
            var userRepo = relational.GetRepository<IUserRepository>();

            // Validate steam ticket
            if (string.IsNullOrEmpty(dto.SteamTicket))
                throw new BadRequest(
                    ResponseCode.SteamAuth_InvalidSteamTicket,
                    $"Steam ticket is invalid, can not authenticate by steam");

            // Validate steam ID
            var steamId = await steamValidator.ValidateTicket(dto.SteamTicket);
            if (string.IsNullOrEmpty(steamId))
                throw new Unauthorized(
                    ResponseCode.SteamAuth_SteamValidationFailed,
                    $"Steam validation was failed, there no such steam ID found");

            // Check existence
            var user = await userRepo.GetBySteamIdAsync(steamId);

            // Steam authentication logic
            string accessToken;
            string refreshToken;
            if (user == null)
            {
                // Apply domain - Create user
                user = new User(
                    id: Guid.NewGuid().ToString(),
                    name: dto.SteamName ?? "Player",
                    preferredLocale: dto.PreferredLocale,
                    role: Role.Player,
                    steamId: steamId
                );

                // Apply domain - Login
                user.UpdateLastLogin();
                (accessToken, refreshToken) = tokenService.Generate(user);

                // Apply persistence
                await relational.BeginTransactionAsync();
                await userRepo.AddAsync(user);
                await relational.CommitAsync();
            }
            else
            {
                // Apply domain - Login
                user.UpdateLastLogin();
                (accessToken, refreshToken) = tokenService.Generate(user);

                // Apply persistence
                await relational.SaveChangesAsync();
            }

            return new TokenDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        #endregion
    }
}