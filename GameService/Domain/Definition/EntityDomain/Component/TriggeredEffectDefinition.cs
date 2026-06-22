using Domain.Abstraction;

namespace Domain.Definition.EntityDomain.Component
{
    public class TriggeredEffectDefinition : ComponentDefinition
    {
        #region Attributes
        #endregion

        #region Properties
        public List<string> EffectDefinitionIDs { get; private set; } = new List<string>();
        #endregion

        protected TriggeredEffectDefinition() : base() { }

        public TriggeredEffectDefinition(
            Guid id, 
            string entityDefinitionId,
            List<string> effectDefinitionIds) : base(id, entityDefinitionId) 
        {
            EffectDefinitionIDs = effectDefinitionIds;
        }

        #region Methods
        #endregion
    }
}