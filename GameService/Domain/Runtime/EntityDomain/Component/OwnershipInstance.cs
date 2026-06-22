using Domain.Abstraction;

namespace Domain.Runtime.EntityDomain.Component
{
    public class OwnershipInstance : ComponentInstance
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        #endregion

        public OwnershipInstance(
            string userID) : base(Guid.Empty)
        {
            UserID = userID;
        }

        #region Methods
        #endregion
    }
}