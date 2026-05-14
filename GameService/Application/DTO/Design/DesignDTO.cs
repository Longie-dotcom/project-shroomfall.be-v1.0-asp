using Application.DTO.Definition;

namespace Application.DTO.Design
{
    public class DefinitionVersionLogDTO
    {
        public string ID { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public long Version { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class DefinitionSnapshotDTO
    {
        public long Version { get; set; }

        public List<AttributeDefinitionDTO> AttributeDefinitions { get; set; } = new();
        public List<AttributeValueDefinitionDTO> AttributeValues { get; set; } = new();
        public List<CharacteristicDefinitionDTO> Characteristics { get; set; } = new();
        public List<EffectDefinitionDTO> Effects { get; set; } = new();


        public List<InventoryDefinitionDTO> Inventories { get; set; } = new();
        public List<InventoryItemDefinitionDTO> InventoryItems { get; set; } = new();
        public List<ItemDefinitionDTO> Items { get; set; } = new();
        public List<ItemEffectDefinitionDTO> ItemEffects { get; set; } = new();

        public List<EntityDefinitionDTO> Entities { get; set; } = new();

        public List<CellDefinitionDTO> Cells { get; set; } = new();
        public List<EntitySpawnRuleDefinitionDTO> EntitySpawnRules { get; set; } = new();
        public List<RoomDefinitionDTO> Rooms { get; set; } = new();
        public List<SpawnAreaDefinitionDTO> SpawnAreas { get; set; } = new();
        public List<TileDefinitionDTO> Tiles { get; set; } = new();

        public List<LocaleDTO> Locales { get; set; } = new();
    }
}