using API.Helper;
using Application.DTO.Connection;
using Application.Features.Abstraction;
using Application.Features.Connection.Commands;
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
        [HttpPost]
        public async Task<IActionResult> CreateSession(
            [FromBody] CreateSessionDTO dto)
        {
            var (userId, steamId, role) = ClaimReader.GetIdentity(User);

            await dispatcher.Send<CreateSessionCommand>(
                new CreateSessionCommand(
                    userId,
                    dto)
            );

            return NoContent();
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> LoadSession(
            [FromBody] LoadSessionDTO dto)
        {
            var (userId, steamId, role) = ClaimReader.GetIdentity(User);

            await dispatcher.Send<LoadSessionCommand>(
                new LoadSessionCommand(
                    userId,
                    dto)
            );

            return NoContent();
        }
        #endregion
    }
}