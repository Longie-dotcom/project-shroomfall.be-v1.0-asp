using Domain.Abstraction;

namespace Domain.Runtime.EntityDomain.Component
{
    public class WorldItemPayloadInstance : ComponentInstance
    {
        #region Attributes
        #endregion

        #region Properties
        public ItemInstance Payload { get; private set; }
        #endregion

        public WorldItemPayloadInstance(
            ItemInstance payload) : base(Guid.Empty)
        {
            Payload = payload;
        }

        #region Methods
        #endregion
    }
}