using API.Helper;
using Application.DTO.Connection;
using Application.DTO.Design;
using Application.Features.Abstraction;
using Application.Features.Design.Commands;
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
        [Authorize]
        [HttpGet("{version}")]
        public async Task<ActionResult<DefinitionSnapshotDTO?>> UserRefresh(
            string version)
        {
            var (userId, steamId, role) = ClaimReader.GetIdentity(User);

            var result = await dispatcher.Send<UserRefreshCommand, DefinitionSnapshotDTO?>(
                new UserRefreshCommand(
                    userId,
                    new UserRefreshDTO
                    {
                        DefinitionVersion = version
                    })
            );

            return Ok(result);
        }

        [Authorize]
        [HttpPost("update")]
        public async Task<IActionResult> UpdateDefinition(
            [FromBody] UpdateDefinitionDTO dto)
        {
            var (userId, steamId, role) = ClaimReader.GetIdentity(User);

            await dispatcher.Send<UpdateDefinitionCommand>(
                new UpdateDefinitionCommand(
                    userId,
                    dto)
            );

            return Ok();
        }
        #endregion
    }
}
