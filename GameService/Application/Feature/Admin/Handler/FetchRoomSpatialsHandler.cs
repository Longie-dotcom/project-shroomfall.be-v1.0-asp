using Application.Feature.Abstraction;
using Application.Feature.Admin.Command;
using Application.Service.WorldService;
using AutoMapper;
using Contract.DTO.Runtime.WorldDomain;

namespace Application.Feature.Admin.Handler
{
    public class FetchRoomSpatialsHandler : IHandler<FetchRoomSpatialsCommand, List<RoomSpatialDTO>>
    {
        #region Attributes
        private readonly IMapper mapper;
        private readonly WorldContext worldContext;
        #endregion

        #region Properties
        #endregion

        public FetchRoomSpatialsHandler(
            IMapper mapper,
            WorldContext worldContext)
        {
            this.mapper = mapper;
            this.worldContext = worldContext;
        }

        #region Methods
        public async Task<List<RoomSpatialDTO>> Handle(
            FetchRoomSpatialsCommand command)
        {
            var runningRoomSptials = worldContext.GetRooms();
            var result = mapper.Map<List<RoomSpatialDTO>>(runningRoomSptials);
            return await Task.FromResult(result);
        }
        #endregion
    }
}