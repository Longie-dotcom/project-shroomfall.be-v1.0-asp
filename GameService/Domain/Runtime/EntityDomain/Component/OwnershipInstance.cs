using Domain.Abstraction;

namespace Domain.Runtime.EntityDomain.Component
{
    public class OwnershipInstance : ComponentInstance
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public string PersonalRoomID { get; }
        #endregion

        public OwnershipInstance(
            string userID,
            string personalRoomID) : base(Guid.Empty)
        {
            UserID = userID;
            PersonalRoomID = personalRoomID;
        }

        #region Methods
        #endregion
    }
}