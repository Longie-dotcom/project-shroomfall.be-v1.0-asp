using Application.DTO;
using Application.Service.Interface;
using AutoMapper;
using Domain.DomainException;
using Domain.IdentityDomain;
using Domain.Shared;
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
        private readonly IMapper mapper;
        #endregion

        #region Properties
        #endregion

        public IdentityService(
            IRelationalUoW relational,
            TokenGenerator tokenGenerator,
            SteamValidator steamValidator,
            IMapper mapper)
        {
            this.relational = relational;
            this.tokenGenerator = tokenGenerator;
            this.steamValidator = steamValidator;
            this.mapper = mapper;
        }

        #region Methods
        public async Task<TokenDTO> SteamAuth(SteamAuthDTO dto)
        {
            // Resolve repositories
            var userRepo = relational.GetRepository<IUserRepository>();

            // Validate steam ticket
            var steamId = await steamValidator.ValidateTicket(dto.SteamTicket);
            if (string.IsNullOrEmpty(steamId))
                throw new BadRequest(FailedCode.Steam_InvalidTicket);

            // Check existence
            var user = await userRepo.GetBySteamIdAsync(steamId);

            // Steam authentication logic
            var accessToken = "";
            var refreshToken = "";
            if (user == null)
            {
                // Apply domain - Create user
                user = new User(
                    id: Guid.NewGuid().ToString(),
                    playerId: Guid.NewGuid().ToString(),
                    name: dto.SteamName ?? "Player",
                    steamId: steamId
                );

                // Apply domain - Login
                user.UpdateLastLogin();
                (accessToken, refreshToken) = GenerateTokens(user);

                // Apply persistence
                await relational.BeginTransactionAsync();
                await userRepo.AddAsync(user);
                await relational.CommitAsync();
            }
            else
            {
                // Apply domain - Login
                user.UpdateLastLogin();
                (accessToken, refreshToken) = GenerateTokens(user);

                // Apply persistence
                await relational.SaveChangesAsync();
            }

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

            // Validate fields
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new BadRequest(FailedCode.User_EmailRequired);

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new BadRequest(FailedCode.User_PasswordRequired);

            // Validate email existence
            var email = dto.Email.Trim().ToLowerInvariant();
            if (await userRepo.EmailExistsAsync(email))
                throw new BadRequest(FailedCode.User_EmailAlreadyExists);

            // Apply domain - Create user
            var user = new User(
                id: Guid.NewGuid().ToString(),
                playerId: Guid.NewGuid().ToString(),
                name: dto.Name ?? "Player",
                email: email
            );
            user.SetPassword(Password.Create(dto.Password));

            // Apply domain - Login and set token
            user.UpdateLastLogin();
            (var accessToken, var refreshToken) = GenerateTokens(user);

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

        public async Task<TokenDTO> Login(
            LoginDTO dto)
        {
            // Resolve repositories
            var userRepo = relational.GetRepository<IUserRepository>();

            // Validate fields
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new BadRequest(FailedCode.User_EmailRequired);

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new BadRequest(FailedCode.User_PasswordRequired);

            // Validate authentication
            var email = dto.Email.Trim().ToLowerInvariant();
            var user = await userRepo.GetByEmailAsync(email);
            if (user == null)
                throw new Unauthorized(FailedCode.User_InvalidCredentials);
            user.VerifyPassword(dto.Password);

            // Apply domain - Login and set token
            user.UpdateLastLogin();
            (var accessToken, var refreshToken) = GenerateTokens(user);

            // Apply persistence
            await relational.SaveChangesAsync();

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
            if (user == null)
                throw new NotFound(FailedCode.User_NotFound);

            if (!user.IsRefreshTokenValid(dto.RefreshToken))
                throw new BadRequest(FailedCode.User_InvalidRefreshToken);

            // Apply domain - Set token
            (var accessToken, var newRefreshToken) = GenerateTokens(user);
            
            // Apply persistence
            await relational.SaveChangesAsync();

            return new TokenDTO
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task<IEnumerable<UserDTO>> GetUser()
        {
            var users = await relational.GetRepository<IUserRepository>()
                .GetAllAsync();

            return mapper.Map<IEnumerable<UserDTO>>(users);
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
                throw new NotFound(FailedCode.User_NotFound);

            // Apply domain - Update profile
            user.UpdateProfile(
                dto.Name,
                dto.Dob,
                dto.Gender
            );

            // Apply persistence
            await relational.SaveChangesAsync();
        }

        private (string access, string refresh) GenerateTokens(User user)
        {
            var access = tokenGenerator.GenerateAccessToken(user.ID, user.SteamID ?? "");
            var refresh = tokenGenerator.GenerateRefreshToken();
            user.SetRefreshToken(refresh, tokenGenerator.GetRefreshTokenExpiry());

            return (access, refresh);
        }
        #endregion
    }
}