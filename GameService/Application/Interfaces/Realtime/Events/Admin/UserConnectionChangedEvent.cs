namespace Application.Interfaces.Realtime.Events.Admin
{
    public class UserConnectionChangedEvent : IEvent
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public int ActiveConnectionCount { get; }
        public DateTime OccurredAt { get; }
        #endregion

        public UserConnectionChangedEvent(
            string userId,
            int activeConnectionCount)
        {
            UserID = userId;
            ActiveConnectionCount = activeConnectionCount;
            OccurredAt = DateTime.Now;
        }

        #region Methods
        #endregion
    }
}