using API.Helper;
using Application.Features.Abstraction;
using Application.Features.Admin.Commands;
using Application.Services.WorldService;
using Contract.DTO.Runtime.WorldDomain;
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
        [HttpPost("room-spatials")]
        public async Task<IActionResult> GetRoomSpatials()
        {
            var (userId, steamId, role) = ClaimReader.GetIdentity(User);

            var result = await dispatcher.Send<FetchRoomSpatialsCommand, List<RoomInstanceDTO>>(
                new FetchRoomSpatialsCommand(userId)
            );

            return Ok(result);
        }

        [Authorize(Roles = nameof(Role.Admin))]
        [HttpPost("room-instance/{roomSpatailId}")]
        public async Task<IActionResult> GetRoomInstance(
            string roomSpatailId)
        {
            var (userId, steamId, role) = ClaimReader.GetIdentity(User);

            var result = await dispatcher.Send<FetchRoomInstanceCommand, RoomInstanceDTO>(
                new FetchRoomInstanceCommand(userId, roomSpatailId)
            );

            return Ok(result);
        }
        #endregion
    }
}