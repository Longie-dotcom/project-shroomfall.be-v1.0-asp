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
        #endregion

        public AppearanceInstance(
            Guid definitionId,
            string skinId,
            HSV skinColor) : base(definitionId)
        {
            SkinID = skinId;
            SkinColor = skinColor;
        }

        #region Methods
        public void UpdateAppearance(
            string skinId,
            HSV skinColor)
        {
            SkinID = skinId;
            SkinColor = skinColor;
        }
        #endregion
    }
}