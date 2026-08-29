using Application.Interface.Realtime.Events;

namespace Application.Interface.Realtime.Events.Design
{
    public class DefinitionUpdatedEvent : IEvent
    {
        #region Attributes
        #endregion

        #region Properties
        public string Key { get; }
        public long Version { get; }
        public DateTime OccurredAt { get; }
        #endregion

        public DefinitionUpdatedEvent(
            string key,
            long version)
        {
            Key = key;
            Version = version;
            OccurredAt = DateTime.Now;
        }

        #region Methods
        #endregion
    }
}