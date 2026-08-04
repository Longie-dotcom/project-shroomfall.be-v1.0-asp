using API.Helper;
using Application.Features.Abstraction;
using Application.Features.Game.Commands;
using Contract.DTO.Feature.Connection.Response;
using Contract.DTO.Feature.Game.Command;
using Contract.DTO.Feature.Game.Response;
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
        [HttpPost("back-home")]
        public async Task<IActionResult> BackHome()
        {
            var (userId, _, _) = ClaimReader.GetIdentity(User);

            var snapshot = await dispatcher.Send<BackHomeCommand, SaveGameDTO>(
                new BackHomeCommand(userId)
            );

            return Ok(snapshot);
        }

        [Authorize]
        [HttpPost("combat-run")]
        public async Task<IActionResult> CreateCombatRun(
            [FromBody] CreateCombatRunDTO dto)
        {
            var (userId, _, _) = ClaimReader.GetIdentity(User);

            var snapshot = await dispatcher.Send<CreateCombatRunCommand, CombatRunDTO>(
                new CreateCombatRunCommand(userId, dto.CombatDefinitionID)
            );

            return Ok(snapshot);
        }

        [Authorize]
        [HttpPost("enter-hub/{hubRoomSpatialId}")]
        public async Task<IActionResult> EnterHub(
            [FromRoute] string hubRoomSpatialId)
        {
            var (userId, _, _) = ClaimReader.GetIdentity(User);

            var snapshot = await dispatcher.Send<EnterHubCommand, SaveGameDTO>(
                new EnterHubCommand(userId, hubRoomSpatialId)
            );

            return Ok(snapshot);
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

        [Authorize]
        [HttpPost("use-item")]
        public async Task<IActionResult> UseItem(
            [FromBody] UseItemDTO dto)
        {
            var (userId, steamId, role) = ClaimReader.GetIdentity(User);

            await dispatcher.Send<UseItemCommand>(
                new UseItemCommand(userId, dto)
            );

            return NoContent();
        }
        #endregion
    }
}