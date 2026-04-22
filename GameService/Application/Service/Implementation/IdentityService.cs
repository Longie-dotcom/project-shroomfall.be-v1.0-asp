using Application.ApplicationException;
using Application.DTO;
using Application.Service.Interface;
using Domain.IdentityDomain;
using Infrastructure.Repository.Interface;
using Infrastructure.Security;

namespace Application.Service.Implementation
{
    public class IdentityService : IIdentityService
    {
        #region Attributes
        private readonly IRelationalUoW relational;
        private readonly TokenGenerator tokenGenerator;
        private readonly SteamValidator steamValidator;
        #endregion

        #region Properties
        #endregion

        public IdentityService(
            IRelationalUoW relational,
            TokenGenerator tokenGenerator,
            SteamValidator steamValidator)
        {
            this.relational = relational;
            this.tokenGenerator = tokenGenerator;
            this.steamValidator = steamValidator;
        }

        #region Methods
        public async Task<TokenDTO> SteamAuth(SteamAuthDTO dto)
        {
            var userRepo = relational.GetRepository<IUserRepository>();

            // Validate steam ticket
            var steamId = await steamValidator.ValidateTicket(dto.SteamTicket);

            if (string.IsNullOrEmpty(steamId))
                throw new BadRequest("Invalid Steam ticket");

            // Check existence
            var user = await userRepo.GetBySteamIdAsync(steamId);

            // Apply domain - Create user
            if (user == null)
            {
                user = new User(
                    id: Guid.NewGuid().ToString(),
                    playerId: Guid.NewGuid().ToString(),
                    name: dto.SteamName ?? "Player",
                    steamId: steamId
                );

                await userRepo.AddAsync(user);
            }

            // Apply domain - Login
            user.UpdateLastLogin();
            var accessToken = tokenGenerator.GenerateAccessToken(user.ID, user.SteamID!);
            var refreshToken = tokenGenerator.GenerateRefreshToken();
            user.SetRefreshToken(refreshToken, tokenGenerator.GetRefreshTokenExpiry());

            // Apply persistence
            await relational.BeginTransactionAsync();
            await relational.CommitAsync();

            return new TokenDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<TokenDTO> Register(
            RegisterDTO dto)
        {
            // Resolve repositories
            var userRepo = relational.GetRepository<IUserRepository>();

            // Validate existence
            if (await userRepo.EmailExistsAsync(dto.Email))
                throw new BadRequest(
                    "Email already exists");

            // Apply domain - Create user
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new BadRequest(
                    "Email required");

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new BadRequest(
                    "Password required");

            var user = new User(
                id: Guid.NewGuid().ToString(),
                playerId: Guid.NewGuid().ToString(),
                name: dto.Name ?? "Player",
                email: dto.Email
            );
            user.SetPassword(Password.Create(dto.Password));

            // Apply domain - Set token
            var accessToken = tokenGenerator.GenerateAccessToken(user.ID, user.SteamID ?? "");
            var refreshToken = tokenGenerator.GenerateRefreshToken();
            user.SetRefreshToken(refreshToken, tokenGenerator.GetRefreshTokenExpiry());

            // Apply persistence
            await userRepo.AddAsync(user);
            await relational.BeginTransactionAsync();
            await relational.CommitAsync();

            return new TokenDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<TokenDTO> Login(
            LoginDTO dto)
        {
            // Resolve repositories
            var userRepo = relational.GetRepository<IUserRepository>();

            // Validate authentication
            var user = await userRepo.GetByEmailAsync(dto.Email);

            if (user == null || !user.VerifyPassword(dto.Password))
                throw new BadRequest(
                    "Invalid credentials");

            // Apply domain - Login and set token
            user.UpdateLastLogin();
            var accessToken = tokenGenerator.GenerateAccessToken(user.ID, user.SteamID ?? "");
            var refreshToken = tokenGenerator.GenerateRefreshToken();
            user.SetRefreshToken(refreshToken, tokenGenerator.GetRefreshTokenExpiry());

            // Apply persistence
            await relational.BeginTransactionAsync();
            await relational.CommitAsync();

            return new TokenDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<TokenDTO> RefreshToken(
            string userId,
            RefreshTokenDTO dto)
        {
            // Resolve repositories
            var userRepo = relational.GetRepository<IUserRepository>();

            // Validate authentication
            var user = await userRepo.GetByIdAsync(userId);

            if (user == null || !user.IsRefreshTokenValid(dto.RefreshToken))
                throw new BadRequest(
                    "Invalid refresh token");

            // Apply domain - Set token
            var accessToken = tokenGenerator.GenerateAccessToken(user.ID, user.SteamID ?? "");
            var newRefreshToken = tokenGenerator.GenerateRefreshToken();
            user.SetRefreshToken(newRefreshToken, tokenGenerator.GetRefreshTokenExpiry());

            // Apply persistence
            await relational.BeginTransactionAsync();
            await relational.CommitAsync();

            return new TokenDTO
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task UpdateProfile(
            string userId,
            UpdateProfileDTO dto)
        {
            // Resolve repositories
            var userRepo = relational.GetRepository<IUserRepository>();

            // Validate existence
            var user = await userRepo.GetByIdAsync(userId);
            
            if (user == null)
                throw new NotFound(
                    "User not found");

            // Apply domain - Update profile
            user.UpdateProfile(
                dto.Name,
                dto.Dob,
                dto.Gender
            );

            // Apply persistence
            await relational.BeginTransactionAsync();
            await relational.CommitAsync();
        }
        #endregion
    }
}