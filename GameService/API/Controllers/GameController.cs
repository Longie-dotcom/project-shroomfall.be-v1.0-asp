using API.Helper;
using Application.Features.Abstraction;
using Application.Features.Game.Commands;
using Contract.DTO.Connection;
using Contract.DTO.Game;
using Contract.DTO.Runtime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        #region Attributes
        private readonly IDispatcher dispatcher;
        #endregion

        #region Properties
        #endregion

        public GameController(
            IDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        #region Methods
        [Authorize]
        [HttpPost("topology/entity/{entityInstanceId}/travel")]
        public async Task<IActionResult> TouchEntity(
            string touchedEntityInstanceId)
        {
            var (userId, steamId, role) = ClaimReader.GetIdentity(User);

            var result = await dispatcher.Send<TouchEntityCommand, RoomSnapshotDTO>(
                new TouchEntityCommand(userId, touchedEntityInstanceId)
            );

            return Ok(result);
        }

        [Authorize]
        [HttpPut("appearance")]
        public async Task<IActionResult> UpdateAppearance(
            [FromBody] UpdatePlayerAppearanceDTO dto)
        {
            var (userId, steamId, role) = ClaimReader.GetIdentity(User);

            await dispatcher.Send<UpdateAppearanceCommand>(
                new UpdateAppearanceCommand(userId, dto)
            );

            return NoContent();
        }
        #endregion
    }
}