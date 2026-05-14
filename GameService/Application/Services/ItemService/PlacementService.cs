using Domain.Common;
using Domain.Definition.ItemDomain;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.ItemDomain;

namespace Application.Services.ItemService
{
    public class PlacementService
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public PlacementService()
        {

        }

        #region Methods
        public void Place(
            CreatureInstance creature, 
            ItemInstance item,
            Item itemDef,
            Vector2 position)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}