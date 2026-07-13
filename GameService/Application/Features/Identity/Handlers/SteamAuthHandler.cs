using Application.Features.Abstraction;
using Application.Features.Identity.Commands;
using Application.Interfaces.Repository.Base;
using Application.Interfaces.Repository.Relational;
using Application.Interfaces.Utility;
using Application.Services.IdentityService;
using Contract.DTO.Feature.Identity.Response;
using Contract.Enum.IdentityDomain;
using Domain.Definition.IdentityDomain;
using Domain.DomainException;
using ResponseCode;

namespace Application.Features.Identity.Handlers
{
    public class SteamAuthHandler : IHandler<SteamAuthCommand, TokenDTO>
    {
        #region Attributes
        private readonly IRelationalUoW relationalUoW;
        private readonly ISteamValidator steamValidator;
        private readonly TokenService tokenService;
        #endregion

        #region Properties
        #endregion

        public SteamAuthHandler(
            IRelationalUoW relationalUoW,
            ISteamValidator steamValidator,
            TokenService tokenService)
        {
            this.relationalUoW = relationalUoW;
            this.steamValidator = steamValidator;
            this.tokenService = tokenService;
        }

        #region Methods
        public async Task<TokenDTO> Handle(
            SteamAuthCommand command)
        {
            var dto = command.DTO;

            // Resolve repository
            var userRepo = relationalUoW.GetRepository<IUserRepository>();

            // Validate steam ticket
            if (string.IsNullOrEmpty(dto.SteamTicket))
                throw new BadRequest(
                    ApplicationCode.IdentityHandlerCode.SteamAuthInvalidSteamTicket,
                    $"Steam ticket is invalid, can not authenticate by steam");

            // Validate steam ID
            var steamId = await steamValidator.ValidateTicket(dto.SteamTicket);
            if (string.IsNullOrEmpty(steamId))
                throw new Unauthorized(
                    ApplicationCode.IdentityHandlerCode.SteamAuthValidationFailed,
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
                    role: Role.Player,
                    steamId: steamId
                );

                // Apply domain - Login
                user.UpdateLastLogin();
                (accessToken, refreshToken) = tokenService.Generate(user);

                // Apply persistence
                await relationalUoW.BeginTransactionAsync();
                await userRepo.AddAsync(user);
                await relationalUoW.CommitAsync();
            }
            else
            {
                // Apply domain - Login
                user.UpdateLastLogin();
                (accessToken, refreshToken) = tokenService.Generate(user);

                // Apply persistence
                await relationalUoW.SaveChangesAsync();
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