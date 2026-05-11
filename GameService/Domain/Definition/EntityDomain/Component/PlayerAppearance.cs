using Domain.Common;

namespace Domain.Definition.EntityDomain.Component
{
    public class PlayerAppearance : Appearance
    {
        #region Attributes
        #endregion

        #region Properties
        public string HairID { get; private set; }
        public string GlassesID { get; private set; }
        public string ShirtID { get; private set; }
        public string PantID { get; private set; }
        public string ShoeID { get; private set; }
        public string EyesID { get; private set; }
        public HSV HairColor { get; private set; }
        public HSV PantColor { get; private set; }
        public HSV EyeColor { get; private set; }
        #endregion

        protected PlayerAppearance()
        {

        }

        public PlayerAppearance(
            string skinId,
            HSV skinColor,
            string hairId,
            string glassesId,
            string shirtId,
            string pantId,
            string shoeId,
            string eyesId,
            HSV hairColor,
            HSV pantColor,
            HSV eyeColor)
            : base(skinId, skinColor)
        {
            HairID = hairId;
            GlassesID = glassesId;
            ShirtID = shirtId;
            PantID = pantId;
            ShoeID = shoeId;
            EyesID = eyesId;

            HairColor = hairColor;
            PantColor = pantColor;
            EyeColor = eyeColor;
        }

        #region Methods
        #endregion
    }
}