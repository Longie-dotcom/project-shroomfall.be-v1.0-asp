namespace Application.Interfaces.Realtime.Events.Admin
{
    public class UserConnectionChangedEvent : IEvent
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public string? ConnectionID { get; }
        public DateTime OccurredAt { get; }
        #endregion

        public UserConnectionChangedEvent(
            string userId,
            string? connectionId)
        {
            UserID = userId;
            ConnectionID = connectionId;
            OccurredAt = DateTime.Now;
        }

        #region Methods
        #endregion
    }
}