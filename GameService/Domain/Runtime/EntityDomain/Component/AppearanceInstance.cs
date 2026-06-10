using Domain.Common;

namespace Domain.Runtime.EntityDomain.Component
{
    public class AppearanceInstance
    {
        #region Attributes
        #endregion

        #region Properties
        public string SkinID { get; private set; }
        public HSV SkinColor { get; private set; }
        public string? HairID { get; private set; } = string.Empty;
        public string? EyesID { get; private set; } = string.Empty;
        public string? ShirtID { get; private set; } = string.Empty;
        public string? PantID { get; private set; } = string.Empty;
        public HSV? HairColor { get; private set; }
        public HSV? PantColor { get; private set; }
        #endregion

        public AppearanceInstance(
            string skinId,
            HSV skinColor,
            string? hairId,
            string? eyesId,
            string? shirtId,
            string? pantId,
            HSV? hairColor,
            HSV? pantColor)
        {
            SkinID = skinId;
            SkinColor = skinColor;
            HairID = hairId;
            EyesID = eyesId;
            ShirtID = shirtId;
            PantID = pantId;
            HairColor = hairColor ?? new HSV(0, 0, 0); // Default to black/transparent if null
            PantColor = pantColor ?? new HSV(0, 0, 0);
        }

        #region Methods
        #endregion
    }
}