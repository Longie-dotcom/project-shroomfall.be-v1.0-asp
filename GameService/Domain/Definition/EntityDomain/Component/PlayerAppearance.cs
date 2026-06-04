using Domain.Common;

namespace Domain.Definition.EntityDomain.Component
{
    public class PlayerAppearance : Appearance
    {
        #region Attributes
        #endregion

        #region Properties
        public string HairID { get; private set; }
        public string EyesID { get; private set; }
        public string ShirtID { get; private set; }
        public string PantID { get; private set; }
        public HSV HairColor { get; private set; }
        public HSV PantColor { get; private set; }
        #endregion

        protected PlayerAppearance()
        {

        }

        public PlayerAppearance(
            string skinId,
            HSV skinColor,
            string hairId,
            string eyesId,
            string shirtId,
            string pantId,
            HSV hairColor,
            HSV pantColor)
            : base(skinId, skinColor)
        {
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