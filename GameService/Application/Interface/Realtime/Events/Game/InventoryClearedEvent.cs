using Application.Interface.Realtime.Events;

namespace Application.Interface.Realtime.Events.Game
{
    public class InventoryClearedEvent : IEvent
    {
        #region Attributes
        #endregion

        #region Properties
        public string EntityInstanceID { get; }
        public DateTime OccurredAt { get; }
        #endregion

        public InventoryClearedEvent(
            string entityInstanceId)
        {
            EntityInstanceID = entityInstanceId;
            OccurredAt = DateTime.UtcNow;
        }

        #region Methods
        #endregion
    }
}