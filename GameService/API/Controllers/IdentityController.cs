using API.Helper;
using Application.DTO.Identity;
using Application.Features.Abstraction;
using Application.Features.Identity.Commands;
using Application.Identity.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        #region Attributes
        private readonly IDispatcher dispatcher;
        #endregion

        #region Properties
        #endregion

        public IdentityController(
            IDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        #region Methods
        [HttpPost("steam")]
        public async Task<IActionResult> SteamAuth(
            [FromBody] SteamAuthDTO dto)
        {
            var result = await dispatcher.Send<SteamAuthCommand, TokenDTO>(
                new SteamAuthCommand(dto)
            );

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterDTO dto)
        {
            var result = await dispatcher.Send<RegisterCommand, TokenDTO>(
                new RegisterCommand(dto)
            );

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginDTO dto)
        {
            var result = await dispatcher.Send<LoginCommand, TokenDTO>(
                new LoginCommand(dto)
            );

            return Ok(result);
        }

        [Authorize]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken(
            [FromBody] RefreshTokenDTO dto)
        {
            var (userId, steamId, role) = ClaimReader.GetIdentity(User);

            var result = await dispatcher.Send<RefreshTokenCommand, TokenDTO>(
                new RefreshTokenCommand(userId, dto)
            );

            return Ok(result);
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(
            [FromBody] UpdateProfileDTO dto)
        {
            var (userId, steamId, role) = ClaimReader.GetIdentity(User);

            await dispatcher.Send<UpdateProfileCommand>(
                new UpdateProfileCommand(userId, dto)
            );

            return NoContent();
        }
        #endregion
    }
}