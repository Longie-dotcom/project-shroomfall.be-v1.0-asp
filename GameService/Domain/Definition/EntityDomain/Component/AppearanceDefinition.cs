using Domain.Abstraction;
using Domain.Common;

namespace Domain.Definition.EntityDomain.Component
{
    public class AppearanceDefinition : ComponentDefinition
    {
        #region Attributes
        #endregion

        #region Properties
        public string SkinID { get; private set; } = string.Empty;
        public HSV SkinColor { get; private set; } = new HSV();
        #endregion

        protected AppearanceDefinition() : base() { }

        public AppearanceDefinition(
            Guid id,
            string entityDefinitionId,
            string skinId,
            HSV skinColor) : base(id, entityDefinitionId)
        {
            SkinID = skinId;
            SkinColor = skinColor;
        }

        #region Methods
        #endregion
    }
}