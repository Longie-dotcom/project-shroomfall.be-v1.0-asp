namespace Application.Interfaces.Realtime.Events.Admin
{
    public class UserSessionChangedEvent : IEvent
    {
        #region Attributes
        #endregion

        #region Properties
        public string UserID { get; }
        public string? PlayerInstanceID { get; }
        public DateTime OccurredAt { get; }
        #endregion

        public UserSessionChangedEvent(
            string userId,
            string? playerInstanceId)
        {
            UserID = userId;
            PlayerInstanceID = playerInstanceId;
            OccurredAt = DateTime.Now;
        }

        #region Methods
        #endregion
    }
}