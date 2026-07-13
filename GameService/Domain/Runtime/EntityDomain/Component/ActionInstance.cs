using Contract.Enum.MetaDomain.Item;
using Domain.Abstraction;
using Domain.Common;

namespace Domain.Runtime.EntityDomain.Component
{
    public class ActionInstance : ComponentInstance
    {
        #region Attributes
        #endregion

        #region Properties
        public string? PendingItemUseID { get; private set; } = string.Empty;
        public Vector2 PendingTargetPosition { get; private set; } = Vector2.Zero;
        public EquipmentSlot? PendingUnequippedSlot { get; private set; }
        public ItemUsageAction ItemUsageAction { get; private set; }
        #endregion

        public ActionInstance() : base(Guid.Empty) { }

        #region Methods
        public void SetItemUseIntent(
            string itemInstanceId, 
            Vector2 targetPosition,
            EquipmentSlot? unequippedSlot,
            ItemUsageAction itemUsageAction)
        {
            PendingItemUseID = itemInstanceId;
            PendingTargetPosition = targetPosition;
            PendingUnequippedSlot = unequippedSlot;
            ItemUsageAction = itemUsageAction;
        }

        public void ClearItemUseIntent()
        {
            PendingItemUseID = null;
            PendingTargetPosition = Vector2.Zero;
            PendingUnequippedSlot = null;
            ItemUsageAction = ItemUsageAction.None;
        }
        #endregion
    }
}