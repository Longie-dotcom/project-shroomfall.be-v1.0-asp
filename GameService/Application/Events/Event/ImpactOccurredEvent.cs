using Application.Events.Abstraction;
using Domain.Common;

namespace Application.Events.Event
{
    public class ImpactOccurredEvent : IEvent
    {
        #region Attributes
        #endregion

        #region Properties
        public string SourceInstanceID { get; }
        public string SourceDefinitionID { get; }
        public string RoomSpatialID { get; }
        public Vector2 Position { get; }
        public List<string> HitTargetInstanceIDs { get; }
        public DateTime OccurredAt { get; }
        #endregion

        public ImpactOccurredEvent(
            string sourceInstanceId,
            string sourceDefinitionId,
            string roomSpatialId,
            Vector2 position,
            List<string> hitTargetInstanceIds)
        {
            SourceInstanceID = sourceInstanceId;
            SourceDefinitionID = sourceDefinitionId;
            RoomSpatialID = roomSpatialId;
            Position = position;
            HitTargetInstanceIDs = hitTargetInstanceIds;
            OccurredAt = DateTime.UtcNow;
        }

        #region Methods
        #endregion
    }
}