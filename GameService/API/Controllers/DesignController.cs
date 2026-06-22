using API.Helper;
using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Contract.DTO.Design;
using Contract.Enum.IdentityDomain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignController : ControllerBase
    {
        #region Attributes
        private readonly IDispatcher dispatcher;
        #endregion

        #region Properties
        #endregion

        public DesignController(
            IDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        #region Methods
        [AllowAnonymous]
        [HttpGet("locale")]
        public async Task<ActionResult<ExistLocalesDTO>> GetLocales()
        {
            var result = await dispatcher.Send<FetchLocaleCommand, ExistLocalesDTO>(
                new FetchLocaleCommand()
            );

            return Ok(result);
        }

        [Authorize]
        [HttpGet("{version}")]
        public async Task<ActionResult<DefinitionSnapshotDTO?>> UserRefresh(
            string version)
        {
            var (userId, steamId, role) = ClaimReader.GetIdentity(User);

            var result = await dispatcher.Send<UserRefreshCommand, DefinitionSnapshotDTO?>(
                new UserRefreshCommand(userId, new UserRefreshDTO { DefinitionVersion = version })
            );

            return Ok(result);
        }

        [Authorize(Roles = nameof(Role.Designer) + "," + nameof(Role.Admin))]
        [HttpPost("update")]
        public async Task<IActionResult> UpdateDefinition(
            [FromBody] UpdateDefinitionDTO dto)
        {
            var (userId, steamId, role) = ClaimReader.GetIdentity(User);

            await dispatcher.Send<UpdateDefinitionCommand>(
                new UpdateDefinitionCommand(userId, dto)
            );

            return Ok();
        }
        #endregion
    }
}
