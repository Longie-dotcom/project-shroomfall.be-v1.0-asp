using Application.Services.EntityService;
using Application.Services.MetaService;
using Application.Systems.Abstraction;
using Contract.Enum.MetaDomain.Effect;
using Domain.Common;

namespace Application.Systems.Queue
{
    public readonly struct MovementCommand : IEntityCommand
    {
        public string EntityInstanceID { get; }
        public CollisionBody Body { get; }
        public Vector2 DesiredPosition { get; }

        public MovementCommand(
            string entityInstanceId,
            CollisionBody body, 
            Vector2 desiredPosition)
        {
            EntityInstanceID = entityInstanceId;
            Body = body;
            DesiredPosition = desiredPosition;
        }
    }

    public readonly struct ItemActionCommand : IEntityCommand
    {
        #region Properties
        public string EntityInstanceID { get; }
        public ItemUsageActionContext Context { get; }
        #endregion

        public ItemActionCommand(
            string entityInstanceId,
            ItemUsageActionContext context)
        {
            EntityInstanceID= entityInstanceId;
            Context = context;
        }
    }

    public struct EntityExpiredCommand : IEntityCommand
    {
        #region Properties
        public string EntityInstanceID { get; }
        #endregion

        public EntityExpiredCommand(
            string entityInstanceId)
        {
            EntityInstanceID = entityInstanceId;
        }
    }

    public class VitalThresholdCommand : IEntityCommand
    {
        #region Properties
        public string EntityInstanceID { get; } = string.Empty;
        public AttributeType Vital { get; }
        public float PreviousValue { get; }
        public float CurrentValue { get; }
        #endregion
    
        public VitalThresholdCommand(
            string entityInstanceId,
            AttributeType vitals,
            float previousValue,
            float currentValue)
        {
            EntityInstanceID = entityInstanceId;
            Vital = vitals;
            PreviousValue = previousValue;
            CurrentValue = currentValue;
        }
    }

    public class EntityDespawnCommand : IEntityCommand
    {
        #region Properties
        public string EntityInstanceID { get; } = string.Empty;
        public bool TriggerDeathLogic { get; }
        #endregion

        public EntityDespawnCommand(
            string entityInstanceId,
            bool triggerDeathLogic)
        {
            EntityInstanceID = entityInstanceId;
            TriggerDeathLogic = triggerDeathLogic;
        }
    }
}