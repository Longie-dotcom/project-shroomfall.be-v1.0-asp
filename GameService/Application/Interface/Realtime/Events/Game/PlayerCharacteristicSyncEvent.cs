using Application.Interface.Realtime.Events;
using Domain.Runtime.EntityDomain.Component;

namespace Application.Interface.Realtime.Events.Game
{
    public class PlayerCharacteristicSyncEvent : IEvent
    {
        #region Attributes
        #endregion

        #region Properties
        public string EntityInstanceID { get; }
        public CharacteristicInstance CharacteristicInstance { get; }
        public DateTime OccurredAt { get; }
        #endregion

        public PlayerCharacteristicSyncEvent(
            string entityInstanceId,
            CharacteristicInstance characteristicInstance)
        {
            EntityInstanceID = entityInstanceId;
            CharacteristicInstance = characteristicInstance;
            OccurredAt = DateTime.UtcNow;
        }

        #region Methods
        #endregion
    }
}