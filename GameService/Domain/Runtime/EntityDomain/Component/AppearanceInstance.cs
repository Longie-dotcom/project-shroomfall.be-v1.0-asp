using Domain.Abstraction;
using Domain.Common;

namespace Domain.Runtime.EntityDomain.Component
{
    public class AppearanceInstance : ComponentInstance
    {
        #region Attributes
        #endregion

        #region Properties
        public string SkinID { get; private set; } = string.Empty;
        public HSV SkinColor { get; private set; } = new HSV();
        public string? HairID { get; private set; } = string.Empty;
        public string? EyesID { get; private set; } = string.Empty;
        public string? ShirtID { get; private set; } = string.Empty;
        public string? PantID { get; private set; } = string.Empty;
        public HSV? HairColor { get; private set; } = new HSV();
        public HSV? PantColor { get; private set; } = new HSV();
        #endregion

        public AppearanceInstance(
            Guid definitionId,
            string skinId,
            HSV skinColor,
            string? hairId,
            string? eyesId,
            string? shirtId,
            string? pantId,
            HSV? hairColor,
            HSV? pantColor) : base(definitionId)
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
        public void UpdateAppearance(
            string skinId,
            HSV skinColor,
            string? hairId = null,
            string? eyesId = null,
            string? shirtId = null,
            string? pantId = null,
            HSV? hairColor = null,
            HSV? pantColor = null)
        {
            SkinID = skinId;
            SkinColor = skinColor;
            HairID = hairId;
            EyesID = eyesId;
            ShirtID = shirtId;
            PantID = pantId;
            HairColor = hairColor;
            PantColor = pantColor;
        }
        #endregion
    }
}