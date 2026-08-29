using Application.Interface.Realtime.Events;
using Contract.Enum.MetaDomain.Item;
using Domain.Runtime.MetaDomain;

namespace Application.Interface.Realtime.Events.Game
{
    public class InventoryItemChangedEvent : IEvent
    {
        #region Attributes
        #endregion

        #region Properties
        public string EntityInstanceID { get; }
        public ItemInstance ItemInstance { get; }
        public ItemInventorySyncEvent ChangeType { get; }
        public DateTime OccurredAt { get; }
        #endregion

        public InventoryItemChangedEvent(
            string entityInstanceId,
            ItemInstance itemInstance,
            ItemInventorySyncEvent changeType)
        {
            EntityInstanceID = entityInstanceId;
            ItemInstance = itemInstance;
            ChangeType = changeType;
            OccurredAt = DateTime.UtcNow;
        }

        #region Methods
        #endregion
    }
}