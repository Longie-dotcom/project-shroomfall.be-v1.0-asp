using Contract.Enum.EntityDomain;
using Contract.Enum.MetaDomain.Item;
using Domain.Definition.LocalizationDomain;

namespace Domain.Definition.MetaDomain
{
    public class ItemDefinition
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; private set; } = string.Empty;
        public ItemType Type { get; private set; }
        public ItemCategory Category { get; private set; }        
        public int? MaxStack { get; private set; }
        public int? MaxDurability { get; private set; }
        public EntityAction? TriggeredAction { get; private set; }
        public ItemPresentationDefinition Presentation { get; private set; }

        public ConsumableConfig? ConsumableConfig { get; private set; }
        public EquippableConfig? EquippableConfig { get; private set; }
        public PlaceableConfig? PlaceableConfig { get; private set; }
        public RangedConfig? RangedConfig { get; private set; }
        public MeleeConfig? MeleeConfig { get; private set; }

        public CostConfig CostConfig { get; private set; }
        #endregion

        protected ItemDefinition() : base() { }

        public ItemDefinition(
            string id,
            ItemType type,
            ItemCategory category,
            int? maxStack,
            int? maxDurability,
            EntityAction? triggeredAction,
            ItemPresentationDefinition presentation,
            ConsumableConfig? consumableConfig,
            EquippableConfig? equippableConfig,
            PlaceableConfig? placeableConfig,
            RangedConfig? rangedConfig,
            MeleeConfig? meleeConfig,
            CostConfig costConfig)
        {
            ID = id;
            Type = type;
            Category = category;
            MaxStack = maxStack;
            MaxDurability = maxDurability;
            TriggeredAction = triggeredAction;
            Presentation = presentation;
            ConsumableConfig = consumableConfig;
            EquippableConfig = equippableConfig;
            PlaceableConfig = placeableConfig;
            RangedConfig = rangedConfig;
            MeleeConfig = meleeConfig;
            CostConfig = costConfig;
        }

        #region Methods
        public void UpdateFields(
            ItemType type,
            ItemCategory category,
            int? maxStack,
            int? maxDurability,
            EntityAction? triggeredAction,
            ConsumableConfig? consumableConfig,
            EquippableConfig? equippableConfig,
            PlaceableConfig? placeableConfig,
            RangedConfig? rangedConfig,
            MeleeConfig? meleeConfig,
            CostConfig costConfig)
        {
            Type = type;
            Category = category;
            MaxStack = maxStack;
            MaxDurability = maxDurability;
            TriggeredAction = triggeredAction;
            ConsumableConfig = consumableConfig;
            EquippableConfig = equippableConfig;
            PlaceableConfig = placeableConfig;
            RangedConfig = rangedConfig;
            MeleeConfig = meleeConfig;
            CostConfig = costConfig;
        }
        #endregion
    }

    public class ItemPresentationDefinition
    {
        #region Attributes
        #endregion

        #region Properties
        public LocalizedText LocalizedText { get; private set; } = new LocalizedText();
        public string? IconID { get; private set; } = string.Empty;
        #endregion

        protected ItemPresentationDefinition() { }

        public ItemPresentationDefinition(
            LocalizedText localizedText,
            string? iconId)
        {
            LocalizedText = localizedText;
            IconID = iconId;
        }

        #region Methods
        #endregion
    }

    public class ConsumableConfig
    {
        public List<string> EffectDefinitionIDs { get; set; } = new List<string>();
    }

    public class EquippableConfig
    {
        public EquipmentSlot Slot { get; set; }
        public List<string> EffectDefinitionIDs { get; set; } = new List<string>();
    }

    public class PlaceableConfig
    {
        public string EntityDefinitionID { get; set; } = string.Empty;
    }

    public class RangedConfig
    {
        public string EntityDefinitionID { get; set; } = string.Empty;
    }

    public class MeleeConfig
    {
        public string EntityDefinitionID { get; set; } = string.Empty;
    }

    public class CostConfig
    {
        public ItemConsumptionMethod Method { get; set; }
    }
}