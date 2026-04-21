using Domain.AttributeDomain.Attribute;
using Domain.Common;
using Domain.WorldDomain.Enum;

namespace Domain.WorldDomain.EntityAggregate
{
    public class PlayerAppearance
    {
        #region Attributes
        #endregion

        #region Properties
        public string HairID { get; set; }
        public string GlassesID { get; set; }
        public string ShirtID { get; set; }
        public string PantID { get; set; }
        public string ShoeID { get; set; }
        public string EyesID { get; set; }
        public string SkinID { get; set; }

        public HSV HairColor { get; set; }
        public HSV PantColor { get; set; }
        public HSV EyeColor { get; set; }
        public HSV SkinColor { get; set; }
        #endregion

        public PlayerAppearance(
            string hairId,
            string glassesId,
            string shirtId,
            string pantId,
            string shoeId,
            string eyesId,
            string skinId,
            HSV hairColor,
            HSV pantColor,
            HSV eyeColor,
            HSV skinColor)
        {
            HairID = hairId;
            GlassesID = glassesId;
            ShirtID = shirtId;
            PantID = pantId;
            ShoeID = shoeId;
            EyesID = eyesId;
            SkinID = skinId;

            HairColor = hairColor;
            PantColor = pantColor;
            EyeColor = eyeColor;
            SkinColor = skinColor;
        }

        #region Methods
        #endregion
    }

    public class Player : Creature
    {
        #region Attributes
        #endregion

        #region Properties
        public PlayerAppearance Appearance { get; private set; } 
        #endregion

        public Player(
            string id,
            EntityType type,
            string roomId,
            string layerId,
            Vector2 position,
            Vector2 direction,
            Characteristic characteristic,
            Stats stats,
            List<Effect> activeEffects,
            PlayerAppearance appearance) : base(
                id,
                type,
                roomId,
                layerId,
                position,
                direction,
                characteristic,
                stats,
                activeEffects)
        {
            Appearance = appearance;
        }

        #region Methods
        #endregion
    }
}
