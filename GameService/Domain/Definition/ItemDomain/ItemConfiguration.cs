using Contract.Enum.ItemDomain;
using Domain.Definition.EntityDomain;
using Domain.DomainException;
using Domain.Shared;

namespace Domain.Definition.ItemDomain
{
    public class ItemConfiguration
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; private set; }
        public string ItemID { get; private set; }
        public string EntityID { get; private set; }
        public ItemConfigurationType Type { get; private set; }

        public Item Item { get; private set; }
        public Entity Entity { get; private set; }
        #endregion

        protected ItemConfiguration()
        {

        }

        public ItemConfiguration(
            string id,
            string itemId,
            string entityId,
            ItemConfigurationType type)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new BadRequest(ResponseCode.ItemConfiguration_InvalidId);

            if (string.IsNullOrWhiteSpace(itemId))
                throw new BadRequest(ResponseCode.ItemConfiguration_InvalidItemId);

            if (string.IsNullOrWhiteSpace(entityId))
                throw new BadRequest(ResponseCode.ItemConfiguration_InvalidEntityId);

            ID = id;
            ItemID = itemId;
            EntityID = entityId;
            Type = type;
        }
    }
}