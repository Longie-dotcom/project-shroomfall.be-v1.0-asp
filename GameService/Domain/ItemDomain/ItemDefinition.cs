using Domain.AttributeDomain.Attribute;
using Domain.ItemDomain.Enum;

namespace Domain.ItemDomain
{
    public class ItemDefinition
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string DisplayTag { get; private set; }
        public ItemType Type { get; private set; }
        public ItemCategory Category { get; private set; }
        public Efficiency Efficiency { get; private set; }
        public List<Effect> Effects { get; private set; } = new();
        #endregion

        public ItemDefinition(
            string id,
            string name,
            string description,
            string displayTag,
            ItemType type,
            ItemCategory category,
            Efficiency efficiency,
            List<Effect> effects)
        {
            ID = id;
            Name = name;
            Description = description;
            DisplayTag = displayTag;
            Type = type;
            Category = category;
            Efficiency = efficiency;
            Effects = effects;
        }

        #region Methods
        #endregion
    }
}
