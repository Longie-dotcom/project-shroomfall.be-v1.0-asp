using Domain.Common;
using Domain.Definition.EntityDomain.Component;
using Domain.Document.EntityDomain.Component;
using Domain.Runtime.EntityDomain.Component;

namespace Domain.Shared
{
    public static class AppearanceMapper
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        #region Methods
        public static AppearanceInstance MapAppearance(
            Appearance def)
        {
            return new AppearanceInstance(
                skinId: def.SkinID,
                skinColor: HSV.Clone(def.SkinColor),
                hairId: def.HairID,
                eyesId: def.EyesID,
                shirtId: def.ShirtID,
                pantId: def.PantID,
                hairColor: HSV.Clone(def.HairColor ?? new HSV(0, 0, 0)),
                pantColor: HSV.Clone(def.PantColor ?? new HSV(0, 0, 0))
            );
        }

        public static HSV MapHSV(
            HSVDocument? docHsv,
            HSV? defaultValue = null)
        {
            if (docHsv == null)
                return defaultValue ?? new HSV(0, 0, 0);

            return new HSV(docHsv.H, docHsv.S, docHsv.V);
        }

        public static AppearanceInstance MapAppearance(
            AppearanceDocument doc)
        {
            return new AppearanceInstance(
                skinId: doc.SkinID,
                skinColor: MapHSV(doc.SkinColor),
                hairId: doc.HairID,
                eyesId: doc.EyesID,
                shirtId: doc.ShirtID,
                pantId: doc.PantID,
                hairColor: MapHSV(doc.HairColor),
                pantColor: MapHSV(doc.PantColor)
            );
        }
        #endregion
    }
}