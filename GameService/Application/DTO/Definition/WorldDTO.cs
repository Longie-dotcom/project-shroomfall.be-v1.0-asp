using Domain.Definition.WorldDomain.Enum;

namespace Application.DTO.Definition
{
    public class CellDefinitionDTO
    {
        public string RoomID { get; set; } = string.Empty;
        public string TileID { get; set; } = string.Empty;
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
    }

    public class EntitySpawnRuleDefinitionDTO
    {
        public string ID { get; set; } = string.Empty;
        public SpawnRuleType Type { get; set; }
        public string RoomID { get; set; } = string.Empty;
        public string EntityID { get; set; } = string.Empty;
    }

    public class RoomDefinitionDTO
    {
        public string ID { get; set; } = string.Empty;
        public RoomType Type { get; set; }
        public LocalizedTextDTO LocalizedText { get; set; } = new LocalizedTextDTO();
    }

    public class SpawnAreaDefinitionDTO
    {
        public string ID { get; set; } = string.Empty;
        public int MinX { get; set; }
        public int MinY { get; set; }
        public int MaxX { get; set; }
        public int MaxY { get; set; }
        public int MinCount { get; set; }
        public int MaxCount { get; set; }
        public float Weight { get; set; }
        public string EntitySpawnRuleID { get; set; } = string.Empty;
    }

    public class TileDefinitionDTO
    {
        public string ID { get; set; } = string.Empty;
        public TileType Type { get; set; }
        public LocalizedTextDTO LocalizedText { get; set; } = new LocalizedTextDTO();
    }
}