using Contract;

namespace Application.Services.WorldService
{
    public class BootstrapService
    {
        #region Attributes
        private readonly InitializationService initializationService;
        private readonly ResidencyService residencyService;
        #endregion

        #region Properties
        #endregion

        public BootstrapService(
            InitializationService initializationService,
            ResidencyService residencyService)
        {
            this.initializationService = initializationService;
            this.residencyService = residencyService;
        }

        #region Methods
        public async Task LoadAsync()
        {
            foreach (var hubId in Constraint.STATIC_HUB_ROOM_SPATIAL_IDS)
            {
                var snapshot = initializationService.InitializeRoom(
                    hubId,
                    hubId,
                    null,
                    null,
                    null);

                residencyService.RegisterRuntimeRoom(snapshot.room);
                residencyService.MarkRoomPermanent(hubId);
            }
        }
        #endregion
    }
}