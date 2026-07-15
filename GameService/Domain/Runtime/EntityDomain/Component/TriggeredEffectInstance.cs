using Domain.Abstraction;

namespace Domain.Runtime.EntityDomain.Component
{
    public class TriggeredEffectInstance : ComponentInstance
    {
        #region Attributes
        #endregion

        #region Properties
        public List<string> EffectDefinitionIDs { get; }
        public string SourceEntityID { get; }
        #endregion

        public TriggeredEffectInstance(
            Guid definitionId, 
            List<string> effectDefinitionIds,
            string sourceEntityId) : base(definitionId)
        {
            EffectDefinitionIDs = effectDefinitionIds;
            SourceEntityID = sourceEntityId;
        }

        #region Methods
        #endregion
    }
}