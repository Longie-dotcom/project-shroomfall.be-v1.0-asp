using API.Helper;
using Application.Features.Abstraction;
using Application.Features.Design.Commands;
using Contract.DTO.Common;
using Contract.DTO.Design;
using Contract.DTO.Domain.Definition;
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
        [Authorize(Roles = nameof(Role.Designer) + "," + nameof(Role.Admin))]
        [HttpGet("effects")]
        public async Task<ActionResult<PagedResponseDTO<EffectDefinitionDTO>>> GetAllEffects(
            [FromQuery] EffectDefinitionQueryDTO queries)
        {
            var (userId, steamId, role) = ClaimReader.GetIdentity(User);

            var result = await dispatcher.Send<FetchEffectDefinitionCommand, PagedResponseDTO<EffectDefinitionDTO>>(
                new FetchEffectDefinitionCommand(userId, queries)
            );

            return Ok(result);
        }

        [Authorize(Roles = nameof(Role.Designer) + "," + nameof(Role.Admin))]
        [HttpGet("entities/{id}")]
        public async Task<ActionResult<EntityDefinitionDetailDTO>> FetchEntityDefinitionDetail(
            [FromRoute] string id)
        {
            var (userId, steamId, role) = ClaimReader.GetIdentity(User);

            var result = await dispatcher.Send<FetchEntityDefinitionDetailCommand, EntityDefinitionDetailDTO?>(
                new FetchEntityDefinitionDetailCommand(id, userId)
            );

            return Ok(result);
        }

        [Authorize(Roles = nameof(Role.Designer) + "," + nameof(Role.Admin))]
        [HttpGet("entities")]
        public async Task<ActionResult<PagedResponseDTO<EntityDefinitionDTO>>> GetAllEntities(
            [FromQuery] EntityDefinitionQueryDTO queries)
        {
            var (userId, steamId, role) = ClaimReader.GetIdentity(User);

            var result = await dispatcher.Send<FetchEntityDefinitionCommand, PagedResponseDTO<EntityDefinitionDTO>>(
                new FetchEntityDefinitionCommand(userId, queries)
            );

            return Ok(result);
        }

        [Authorize(Roles = nameof(Role.Designer) + "," + nameof(Role.Admin))]
        [HttpGet("items")]
        public async Task<ActionResult<PagedResponseDTO<ItemDefinitionDTO>>> GetAllItems(
            [FromQuery] ItemDefinitionQueryDTO queries)
        {
            var (userId, steamId, role) = ClaimReader.GetIdentity(User);

            var result = await dispatcher.Send<FetchItemDefinitionCommand, PagedResponseDTO<ItemDefinitionDTO>>(
                new FetchItemDefinitionCommand(userId, queries)
            );

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("locale")]
        public async Task<ActionResult<ExistLocalesDTO>> GetLocales()
        {
            var result = await dispatcher.Send<FetchLocaleCommand, ExistLocalesDTO>(
                new FetchLocaleCommand()
            );

            return Ok(result);
        }

        [Authorize(Roles = nameof(Role.Designer) + "," + nameof(Role.Admin))]
        [HttpPost("definition")]
        public async Task<IActionResult> UpdateDefinition(
            [FromBody] UpdateDefinitionDTO dto)
        {
            var (userId, steamId, role) = ClaimReader.GetIdentity(User);

            await dispatcher.Send<UpdateDefinitionCommand>(
                new UpdateDefinitionCommand(userId, dto)
            );

            return Ok();
        }

        [Authorize(Roles = nameof(Role.Designer) + "," + nameof(Role.Admin))]
        [HttpPost("effect-definition")]
        public async Task<IActionResult> UpsertEffectDefinition(
            [FromBody] UpsertEffectDefinitionDTO dto)
        {
            var (userId, steamId, role) = ClaimReader.GetIdentity(User);

            await dispatcher.Send<UpsertEffectDefinitionCommand>(
                new UpsertEffectDefinitionCommand(userId, dto)
            );

            return Ok();
        }

        [Authorize(Roles = nameof(Role.Designer) + "," + nameof(Role.Admin))]
        [HttpPost("entity-definition")]
        public async Task<IActionResult> UpsertEntityDefinition(
            [FromBody] UpsertEntityDefinitionDTO dto)
        {
            var (userId, steamId, role) = ClaimReader.GetIdentity(User);

            await dispatcher.Send<UpsertEntityDefinitionCommand>(
                new UpsertEntityDefinitionCommand(userId, dto)
            );

            return Ok();
        }

        [Authorize(Roles = nameof(Role.Designer) + "," + nameof(Role.Admin))]
        [HttpPost("item-definition")]
        public async Task<IActionResult> UpsertItemDefinition(
            [FromBody] UpsertItemDefinitionDTO dto)
        {
            var (userId, steamId, role) = ClaimReader.GetIdentity(User);

            await dispatcher.Send<UpsertItemDefinitionCommand>(
                new UpsertItemDefinitionCommand(userId, dto)
            );

            return Ok();
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
        #endregion
    }
}
