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
        #endregion

        protected Appearance() 
        { 
        
        }

        public Appearance(
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