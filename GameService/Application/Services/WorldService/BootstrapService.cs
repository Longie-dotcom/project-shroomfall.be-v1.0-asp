using Application.Context;
using Application.Persistence;

namespace Application.Services.WorldService
{
    public class BootstrapService
    {
        #region Attributes
        private readonly RoomConnectionPersistence roomConnectionPersistence;
        private readonly WorldContext worldContext;
        #endregion

        #region Properties
        #endregion

        public BootstrapService(
            RoomConnectionPersistence roomConnectionPersistence,
            WorldContext worldContext)
        {
            this.roomConnectionPersistence = roomConnectionPersistence;
            this.worldContext = worldContext;
        }

        #region Methods
        public async Task LoadAsync()
        {
            // Load existed connection topology
            var connections = await roomConnectionPersistence.LoadAsync();

            connections.ForEach(c => worldContext.AddConnection(c));
        }
        #endregion
    }
}