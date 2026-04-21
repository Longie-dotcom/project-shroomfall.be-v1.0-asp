using Application.DTO;

namespace Application.Service.Interface
{
    public interface IIdentityService
    {
        Task<TokenDTO> SteamAuth(
            SteamAuthDTO dto);

        Task<TokenDTO> Register(
            RegisterDTO dto);

        Task<TokenDTO> Login(
            LoginDTO dto);

        Task<TokenDTO> RefreshToken(
            string userId,
            RefreshTokenDTO dto);

        Task UpdateProfile(
            string userId,
            UpdateProfileDTO dto);
    }
}
