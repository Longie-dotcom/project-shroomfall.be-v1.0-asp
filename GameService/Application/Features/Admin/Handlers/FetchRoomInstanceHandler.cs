using Application.Features.Abstraction;
using Application.Features.Admin.Commands;
using Application.Services.WorldService;
using AutoMapper;
using Contract.DTO.Runtime.WorldDomain;

namespace Application.Features.Admin.Handlers
{
    public class FetchRoomInstanceHandler : IHandler<FetchRoomInstanceCommand, RoomInstanceDTO>
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
        public async Task<RoomInstanceDTO> Handle(
            FetchRoomInstanceCommand command)
        {
            var roomInstance = residencyService.TryGetRoomInstance(command.RoomSpatialID);
            if (roomInstance == null)
                return new RoomInstanceDTO();

            var result = mapper.Map<RoomInstanceDTO>(roomInstance);
            return await Task.FromResult(result);
        }
        #endregion
    }
}