using Domain.Abstraction;

namespace Domain.Definition.EntityDomain.Component
{
    public class AIDefinition : ComponentDefinition
    {
        #region Attributes
        #endregion

        #region Properties
        public float LeashDistance { get; private set; }
        public float AggroRadius { get; private set; }
        public float ThinkInterval { get; private set; }
        public bool IsAIControlled { get; private set; }
        #endregion

        protected AIDefinition() : base() { }

        public AIDefinition(
            Guid id,
            string entityDefinitionId,
            float leashDistance,
            float aggroRadius,
            float thinkInterval,
            bool isAIControlled) : base(id, entityDefinitionId)
        {
            LeashDistance = leashDistance;
            AggroRadius = aggroRadius;
            ThinkInterval = thinkInterval;
            IsAIControlled = isAIControlled;
        }

        #region Methods
        #endregion
    }
}