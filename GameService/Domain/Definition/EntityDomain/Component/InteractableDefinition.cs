using Contract.Enum.EntityDomain;
using Domain.Abstraction;

namespace Domain.Definition.EntityDomain.Component
{
    public class InteractableDefinition : ComponentDefinition
    {
        #region Attributes
        public WorldObjectInteractionType Type { get; set; }
        #endregion

        #region Properties
        #endregion

        protected InteractableDefinition() : base() { }

        public InteractableDefinition(
            Guid id,
            string entityDefinitionId,
            WorldObjectInteractionType type) : base(id, entityDefinitionId)
        {
            Type = type;
        }

        #region Methods
        #endregion
    }
}