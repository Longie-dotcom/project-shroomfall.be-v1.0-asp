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

        // Configuration per type
        public SpawnEntityConfig? SpawnEntityConfig { get; private set; }
        public ApplyEffectConfig? ApplyEffectConfig { get; private set; }
        public EquipConfig? EquipConfig { get; private set; }

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
            SpawnEntityConfig? spawnEntityConfig,
            ApplyEffectConfig? applyEffectConfig,
            EquipConfig? equipConfig,
            CostConfig costConfig)
        {
            ID = id;
            Type = type;
            Category = category;
            MaxStack = maxStack;
            MaxDurability = maxDurability;
            TriggeredAction = triggeredAction;
            Presentation = presentation;
            SpawnEntityConfig = spawnEntityConfig;
            ApplyEffectConfig = applyEffectConfig;
            EquipConfig = equipConfig;
            CostConfig = costConfig;
        }

        #region Methods
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

    public class SpawnEntityConfig
    {
        public string EntityDefinitionID { get; set; } = string.Empty;
        public SpawnTargetType TargetType { get; set; } = SpawnTargetType.WorldPosition;
        public float MaxRange { get; set; }
    }

    public class ApplyEffectConfig
    {
        public List<string> EffectDefinitionIDs { get; set; } = new List<string>();
    }

    public class EquipConfig
    {
        public EquipmentSlot Slot { get; set; }
    }

    public class CostConfig
    {
        public ItemConsumptionMethod Method { get; set; }
        public int Value { get; set; } = 1;
    }
}