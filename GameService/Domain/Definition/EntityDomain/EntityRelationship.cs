using Contract.Enum.EntityDomain;
using Domain.DomainException;
using Domain.Shared;

namespace Domain.Definition.EntityDomain
{
    public class EntityRelationship
    {
        #region Attributes
        #endregion

        #region Properties
        public string SourceEntityID { get; private set; }
        public string TargetEntityID { get; private set; }
        public EntityRelationshipType Type { get; private set; }
        #endregion

        public EntityRelationship(
            string sourceEntityID,
            string targetEntityID,
            EntityRelationshipType type)
        {
            if (string.IsNullOrWhiteSpace(sourceEntityID))
                throw new BadRequest(ResponseCode.Relationship_InvalidSource);

            if (string.IsNullOrWhiteSpace(targetEntityID))
                throw new BadRequest(ResponseCode.Relationship_InvalidTarget);

            if (sourceEntityID == targetEntityID)
                throw new BadRequest(ResponseCode.Relationship_SameEntity);

            SourceEntityID = sourceEntityID;
            TargetEntityID = targetEntityID;
            Type = type;
        }

        #region Methods
        #endregion
    }
}