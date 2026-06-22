using Domain.Abstraction;

namespace Domain.Definition.EntityDomain.Component
{
    public class PortalDefinition : ComponentDefinition
    {
        #region Attributes
        #endregion

        #region Properties
        public float LocalTriggerOffsetX { get; private set; }
        public float LocalTriggerOffsetY { get; private set; }
        public float TriggerWidth { get; private set; } = 1f;
        public float TriggerHeight { get; private set; } = 1f;
        #endregion

        protected PortalDefinition() : base() { }

        public PortalDefinition(
            Guid id,
            string entityDefinitionId,
            float localOffsetX,
            float localOffsetY) : base(id, entityDefinitionId)
        {
            LocalTriggerOffsetX = localOffsetX;
            LocalTriggerOffsetY = localOffsetY;
        }

        #region Methods
        #endregion
    }
}