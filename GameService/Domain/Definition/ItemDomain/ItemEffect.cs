using Domain.Definition.AttributeDomain;
using Domain.DomainException;
using Domain.Shared;

namespace Domain.Definition.ItemDomain
{
    public class ItemEffect
    {
        #region Attributes
        #endregion

        #region Properties
        public string ItemID { get; private set; }
        public string EffectID { get; private set; }

        public Item Item { get; private set; }
        public Effect Effect { get; private set; }
        #endregion

        protected ItemEffect() 
        { 
        
        }

        public ItemEffect(
            string itemId,
            string effectId)
        {
            if (string.IsNullOrWhiteSpace(itemId))
                throw new BadRequest(ResponseCode.ItemEffect_InvalidItemId);

            if (string.IsNullOrWhiteSpace(effectId))
                throw new BadRequest(ResponseCode.ItemEffect_InvalidEffectId);

            ItemID = itemId;
            EffectID = effectId;
        }

        #region Methods
        #endregion
    }
}