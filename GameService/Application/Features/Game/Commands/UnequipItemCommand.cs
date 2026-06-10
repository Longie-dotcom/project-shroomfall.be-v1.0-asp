using Contract.Enum.EntityDomain;

namespace Application.Features.Game.Commands
{
    public class UnequipItemCommand
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public EquipmentSlot Slot { get; }
        #endregion

        public UnequipItemCommand(
            string userId,
            EquipmentSlot slot)
        {
            UserID = userId;
            Slot = slot;
        }

        #region Methods
        #endregion
    }
}