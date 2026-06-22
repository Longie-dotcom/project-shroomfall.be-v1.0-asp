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
        public string? HairID { get; private set; }
        public string? EyesID { get; private set; }
        public string? ShirtID { get; private set; }
        public string? PantID { get; private set; }
        public HSV HairColor { get; private set; } = new HSV();
        public HSV PantColor { get; private set; } = new HSV();
        #endregion

        protected AppearanceDefinition() : base() { }

        public AppearanceDefinition(
            Guid id,
            string entityDefinitionId,
            string skinId,
            HSV skinColor,
            string? hairId = null,
            string? eyesId = null,
            string? shirtId = null,
            string? pantId = null,
            HSV? hairColor = null,
            HSV? pantColor = null) : base(id, entityDefinitionId)
        {
            SkinID = skinId;
            SkinColor = skinColor;
            HairID = hairId;
            EyesID = eyesId;
            ShirtID = shirtId;
            PantID = pantId;
            HairColor = hairColor ?? new HSV();
            PantColor = pantColor ?? new HSV();
        }

        #region Methods
        #endregion
    }
}