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
        #endregion

        public ActionInstance() : base(Guid.Empty) { }

        #region Methods
        public void SetItemUseIntent(
            string itemInstanceId, 
            Vector2 targetPosition)
        {
            PendingItemUseID = itemInstanceId;
            PendingTargetPosition = targetPosition;
        }

        public void ClearItemUseIntent()
        {
            PendingItemUseID = null;
            PendingTargetPosition = Vector2.Zero;
        }
        #endregion
    }
}