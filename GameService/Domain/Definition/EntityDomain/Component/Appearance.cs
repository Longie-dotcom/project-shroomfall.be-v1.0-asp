using Domain.Common;

namespace Domain.Definition.EntityDomain.Component
{
    public class Appearance
    {
        #region Attributes
        #endregion

        #region Properties
        public string SkinID { get; private set; }
        public HSV SkinColor { get; private set; }
        public string? HairID { get; private set; }
        public string? EyesID { get; private set; }
        public string? ShirtID { get; private set; }
        public string? PantID { get; private set; }
        public HSV? HairColor { get; private set; }
        public HSV? PantColor { get; private set; }
        #endregion

        protected Appearance() 
        { 
        
        }

        public Appearance(
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

        #region Methods
        #endregion
    }
}