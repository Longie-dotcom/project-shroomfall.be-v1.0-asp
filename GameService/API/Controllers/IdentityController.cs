using API.Helper;
using Application.DTO;
using Application.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        #region Attributes
        private readonly IIdentityService identityService;
        #endregion

        #region Properties
        #endregion

        public IdentityController(IIdentityService identityService)
        {
            this.identityService = identityService;
        }

        #region Methods
        [HttpPost("steam")]
        public async Task<IActionResult> SteamAuth([FromBody] SteamAuthDTO dto)
        {
            var result = await identityService.SteamAuth(dto);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            var result = await identityService.Register(dto);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var result = await identityService.Login(dto);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO dto)
        {
            var (userId, _) = ClaimReader.GetIdentity(User);

            var result = await identityService.RefreshToken(userId, dto);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO dto)
        {
            var (userId, _) = ClaimReader.GetIdentity(User);

            await identityService.UpdateProfile(userId, dto);
            return NoContent();
        }
        #endregion
    }
}