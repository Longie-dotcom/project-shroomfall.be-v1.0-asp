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
            //foreach (var hub in Constraint.STATIC_HUB_ROOM_MAPS)
            //{
            //    var snapshot = initializationService.InitializeRoom(
            //        hub.DefinitionKey,
            //        hub.SpatialId,
            //        null,
            //        null,
            //        null);

            //    residencyService.RegisterRuntimeRoom(snapshot.room);
            //    residencyService.MarkRoomPermanent(hub.SpatialId);
            //}
        }
        #endregion
    }
}