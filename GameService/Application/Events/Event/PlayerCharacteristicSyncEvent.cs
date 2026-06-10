using Application.Events.Abstraction;
using Contract.DTO.Runtime;

namespace Application.Events.Event
{
    public class PlayerCharacteristicSyncEvent : IEvent
    {
        #region Attributes
        #endregion

        #region Properties
        public string EntityInstanceID { get; }
        public CharacteristicRuntimeDTO CharacteristicRuntime { get; }
        public DateTime OccurredAt { get; }
        #endregion

        public PlayerCharacteristicSyncEvent(
            string entityInstanceId,
            CharacteristicRuntimeDTO characteristicRuntime,
            DateTime occurredAt)
        {
            EntityInstanceID = entityInstanceId;
            CharacteristicRuntime = characteristicRuntime;
            OccurredAt = DateTime.UtcNow;
        }

        #region Methods
        #endregion
    }
}