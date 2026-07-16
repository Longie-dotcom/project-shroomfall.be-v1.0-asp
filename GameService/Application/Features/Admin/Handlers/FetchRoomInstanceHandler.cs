using Application.Features.Abstraction;
using Application.Features.Admin.Commands;
using Application.Services.WorldService;
using AutoMapper;
using Contract.DTO.Feature.Admin.Response;

namespace Application.Features.Admin.Handlers
{
    public class FetchRoomInstanceHandler : IHandler<FetchRoomInstanceCommand, List<RoomInstanceDTO>>
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly ResidencyService residencyService;
        #endregion

        #region Properties
        #endregion

        public FetchRoomInstanceHandler(
            IMapper mapper,
            ResidencyService residencyService)
        {
            this.mapper = mapper;
            this.residencyService = residencyService;
        }

        #region Methods
        public async Task<List<RoomInstanceDTO>> Handle(
            FetchRoomInstanceCommand command)
        {
            var runningRooms = residencyService.GetRunningRoomInstances();

            var result = mapper.Map<List<RoomInstanceDTO>>(runningRooms);

            return await Task.FromResult(result);
        }
        #endregion
    }
}