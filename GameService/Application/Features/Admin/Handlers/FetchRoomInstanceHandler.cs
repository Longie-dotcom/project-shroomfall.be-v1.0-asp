using Application.Features.Abstraction;
using Application.Features.Admin.Commands;
using Application.Services.WorldService;
using Contract;

namespace Application.Features.Admin.Handlers
{
    public class FetchRoomInstanceHandler : IHandler<FetchRoomInstanceCommand, List<RoomInstance>>
    {
        #region Attributes
        private readonly ResidencyService residencyService;
        #endregion

        #region Properties
        #endregion

        public FetchRoomInstanceHandler(
            ResidencyService residencyService)
        {
            this.residencyService = residencyService;
        }

        #region Methods
        public async Task<List<RoomInstance>> Handle(
            FetchRoomInstanceCommand command)
        {
            var runningRooms = residencyService.GetRunningRoomInstances();

            return await Task.FromResult(runningRooms);
        }
        #endregion
    }
}