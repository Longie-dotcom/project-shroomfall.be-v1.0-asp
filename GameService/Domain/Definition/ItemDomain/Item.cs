using Contract.Enum.ItemDomain;
using Domain.Definition.LocalizationDomain;
using Domain.DomainException;
using Domain.Shared;

namespace Domain.Definition.ItemDomain
{
    public class Item
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; private set; }
        public ItemType Type { get; private set; }
        public LocalizedText LocalizedText { get; private set; }
        public ItemCategory Category { get; private set; }
        public int? Durability { get; private set; }
        public bool Stackable { get; set; }
        public string? CharacteristicID { get; private set; }

        public ICollection<ItemConfiguration> Configurations { get; private set; } = new List<ItemConfiguration>();
        public ICollection<ItemEffect> Effects { get; private set; } = new List<ItemEffect>();
        #endregion

        protected Item() 
        { 
        
        }

        public Item(
            string id,
            ItemType type,
            LocalizedText localizedText,
            ItemCategory category,
            int? durability,
            bool stackable,
            string? characteristicId)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new BadRequest(ResponseCode.Item_InvalidId);

            if (string.IsNullOrWhiteSpace(localizedText.NameKey))
                throw new BadRequest(ResponseCode.Item_InvalidName);

            if (string.IsNullOrWhiteSpace(localizedText.DescriptionKey))
                throw new BadRequest(ResponseCode.Item_InvalidDescription);

            if (durability.HasValue && durability.Value < 0)
                throw new BadRequest(ResponseCode.Item_InvalidDurability);

            if (stackable && durability.HasValue)
                throw new BadRequest(ResponseCode.Item_InvalidStackableDurability);

            ID = id;
            Type = type;
            LocalizedText = localizedText;
            Category = category;
            Durability = durability;
            Stackable = stackable;
            CharacteristicID = characteristicId;
        }

        #region Methods
        #endregion
    }
}
