using Contract.Enum.EntityDomain;
using Domain.Abstraction;

namespace Domain.Definition.EntityDomain.Component
{
    public class EntityRelationshipDefinition : ComponentDefinition
    {
        #region Attributes
        #endregion

        #region Properties
        public string SourceEntityDefinitionID { get; private set; } = string.Empty;
        public string TargetEntityDefinitionID { get; private set; } = string.Empty;
        public EntityRelationshipType Type { get; private set; }
        #endregion

        protected EntityRelationshipDefinition() : base() { }

        public EntityRelationshipDefinition(
            Guid id,
            string entityDefinitionId,
            string sourceEntityDefinitionId,
            string targetEntityDefinitionId,
            EntityRelationshipType type) : base(id, entityDefinitionId)
        {
            SourceEntityDefinitionID = sourceEntityDefinitionId;
            TargetEntityDefinitionID = targetEntityDefinitionId;
            Type = type;
        }

        #region Methods
        #endregion
    }
}