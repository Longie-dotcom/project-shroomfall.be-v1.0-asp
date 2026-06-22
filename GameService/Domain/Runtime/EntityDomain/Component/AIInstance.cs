using Contract.Enum.EntityDomain;
using Domain.Abstraction;

namespace Domain.Runtime.EntityDomain.Component
{
    public class AIInstance : ComponentInstance
    {
        #region Attributes
        #endregion

        #region Properties
        // State Management
        public AIState AIState { get; set; } = AIState.Idle;
        public string? TargetEntityId { get; set; }
        public bool IsAIControlled { get; set; } = true;

        // Timers (To keep AI performance efficient)
        public float ThinkCooldownRemaining { get; set; }
        public float AttackTimer { get; set; }

        // Configuration (Behavioral limits)
        public float LeashDistance { get; set; } = 10.0f;
        public float AggroRadius { get; set; } = 5.0f;
        #endregion

        public AIInstance(
            Guid definitionId,
            float leashDistance,
            float aggroRadius,
            bool isAIControlled,
            float thinkInterval) : base(definitionId)
        {
            LeashDistance = leashDistance;
            AggroRadius = aggroRadius;
            IsAIControlled = isAIControlled;
            ThinkCooldownRemaining = thinkInterval;
        }

        #region Methods
        #endregion
    }
}