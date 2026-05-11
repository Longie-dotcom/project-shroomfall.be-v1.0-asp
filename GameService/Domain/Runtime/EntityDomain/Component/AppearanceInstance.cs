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
        #endregion

        public AppearanceInstance(
            string skinId,
            HSV skinColor)
        {
            SkinID = skinId;
            SkinColor = skinColor;
        }

        #region Methods
        #endregion
    }
}