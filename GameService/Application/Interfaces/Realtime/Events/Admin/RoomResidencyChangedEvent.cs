namespace Application.Interfaces.Realtime.Events.Admin
{
    public class RoomResidencyChangedEvent : IEvent
    {
        #region Attributes
        #endregion

        #region Properties
        public string RoomSpatialID { get; }
        public string PreviousState { get; }
        public string NewState { get; }
        public DateTime OccurredAt { get; }
        #endregion

        public RoomResidencyChangedEvent(
            string roomSpatialId,
            string previousState,
            string newState)
        {
            RoomSpatialID = roomSpatialId;
            PreviousState = previousState;
            NewState = newState;
            OccurredAt = DateTime.UtcNow;
        }

        #region Methods
        #endregion
    }
}