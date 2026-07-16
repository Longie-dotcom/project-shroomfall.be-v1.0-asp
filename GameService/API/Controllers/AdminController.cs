using API.Helper;
using Application.Features.Abstraction;
using Application.Features.Admin.Commands;
using Application.Services.WorldService;
using Contract.DTO.Feature.Admin.Response;
using Contract.Enum.IdentityDomain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        #region Attributes
        private readonly IDispatcher dispatcher;
        #endregion

        #region Properties
        #endregion

        public AdminController(
            IDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        #region Methods

        [Authorize(Roles = nameof(Role.Admin))]
        [HttpPost("room-instances")]
        public async Task<IActionResult> GetRoomInstances()
        {
            var (userId, steamId, role) = ClaimReader.GetIdentity(User);

            var result = await dispatcher.Send<FetchRoomInstanceCommand, List<RoomInstanceDTO>>(
                new FetchRoomInstanceCommand(userId)
            );

            return Ok(result);
        }
        #endregion
    }
}