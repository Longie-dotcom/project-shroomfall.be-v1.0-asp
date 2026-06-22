using Domain.Abstraction;

namespace Domain.Runtime.EntityDomain.Component
{
    public class TriggeredEffectInstance : ComponentInstance
    {
        #region Attributes
        #endregion

        #region Properties
        public List<string> EffectDefinitionIDs { get; }
        #endregion

        public TriggeredEffectInstance(
            Guid definitionId, 
            List<string> effectDefinitionIds) : base(definitionId)
        {
            EffectDefinitionIDs = effectDefinitionIds;
        }

        #region Methods
        #endregion
    }
}