using API.Helper;
using Application.DTO.Connection;
using Application.Features.Abstraction;
using Application.Features.Connection.Commands;
using Application.Features.Connection.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConnectController : ControllerBase
    {
        #region Attributes
        private readonly IDispatcher dispatcher;
        #endregion

        #region Properties
        #endregion

        public ConnectController(
            IDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        #region Methods
        [Authorize]
        [HttpGet("sessions")]
        public async Task<IActionResult> FetchSessions()
        {
            var (userId, steamId, role) = ClaimReader.GetIdentity(User);

            var result = await dispatcher.Send<FetchSessionCommand, ExistedSessionDTO>(
                new FetchSessionCommand(userId)
            );

            return Ok(result);
        }

        [Authorize]
        [HttpPost("session")]
        public async Task<IActionResult> CreateSession(
            [FromBody] CreateSessionDTO dto)
        {
            var (userId, steamId, role) = ClaimReader.GetIdentity(User);

            var result = await dispatcher.Send<CreateSessionCommand, ExistedSessionEntryDTO>(
                new CreateSessionCommand(userId, dto)
            );

            return Ok(result);
        }

        [Authorize]
        [HttpPost("session/load")]
        public async Task<IActionResult> LoadSession(
            [FromBody] LoadSessionDTO dto)
        {
            var (userId, steamId, role) = ClaimReader.GetIdentity(User);

            var result = await dispatcher.Send<LoadSessionCommand, SaveGameDTO>(
                new LoadSessionCommand(userId, dto)
            );

            return Ok(result);
        }

        [Authorize]
        [HttpPatch("room/{newRoomSpatialId}")]
        public async Task<IActionResult> ChangeRoom(
            string newRoomSpatialId)
        {
            var (userId, steamId, role) = ClaimReader.GetIdentity(User);

            var result = await dispatcher.Send<ChangeRoomCommand, RoomSnapshotDTO>(
                new ChangeRoomCommand(userId, newRoomSpatialId)
            );

            return Ok(result);
        }
        #endregion
    }
}