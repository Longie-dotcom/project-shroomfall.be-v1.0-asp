using Contract.Enum.WorldDomain;
using Domain.Definition.EntityDomain;
using Domain.Definition.LocalizationDomain;

namespace Domain.Definition.WorldDomain
{
    public class RoomDefinition
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; private set; } = string.Empty;
        public RoomType Type { get; private set; }
        public RoomPresentationDefinition Presentation { get; private set; }

        public ICollection<Cell> Cells {  get; private set; } = new List<Cell>();
        public ICollection<EntitySpawnRule> EntitySpawnRules { get; private set; } = new List<EntitySpawnRule>();
        #endregion

        protected RoomDefinition() { }

        public RoomDefinition(
            string id,
            RoomType type,
            RoomPresentationDefinition presentation)
        {
            ID = id;
            Type = type;
            Presentation = presentation;
        }

        #region Methods
        #endregion
    }

    public class RoomPresentationDefinition
    {
        #region Attributes
        #endregion

        #region Properties
        public LocalizedText LocalizedText { get; private set; } = new LocalizedText();
        public string? IconID { get; private set; } = string.Empty;
        #endregion

        protected RoomPresentationDefinition() { }

        public RoomPresentationDefinition(
            LocalizedText localizedText,
            string? iconId)
        {
            LocalizedText = localizedText;
            IconID = iconId;
        }

        #region Methods
        #endregion
    }

    public class EntitySpawnRule
    {
        #region Attributes
        #endregion

        #region Properties
        public Guid ID { get; private set; }
        public SpawnRuleType Type { get; private set; }
        public int MinX { get; private set; }
        public int MinY { get; private set; }
        public int MaxX { get; private set; }
        public int MaxY { get; private set; }
        public int MinCount { get; private set; }
        public int MaxCount { get; private set; }
        public string RoomDefinitionID { get; private set; } = string.Empty;
        public string EntityDefinitionID { get; private set; } = string.Empty;

        public RoomDefinition RoomDefinition { get; private set; }
        public EntityDefinition EntityDefinition { get; private set; }
        #endregion

        protected EntitySpawnRule() { }

        public EntitySpawnRule(
            Guid id,
            SpawnRuleType type,
            int minX,
            int minY,
            int maxX,
            int maxY,
            int minCount,
            int maxCount,
            string roomDefinitionId,
            string entityDefinitionId)
        {
            ID = id;
            Type = type;
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
            MinCount = minCount;
            MaxCount = maxCount;
            RoomDefinitionID = roomDefinitionId;
            EntityDefinitionID = entityDefinitionId;
        }

        #region Methods
        #endregion
    }

    public class Cell
    {
        #region Attributes
        #endregion

        #region Properties
        public string RoomDefinitionID { get; private set; } = string.Empty;
        public CellType Type { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Z { get; private set; }

        public RoomDefinition RoomDefinition { get; private set; }
        #endregion

        protected Cell() { }

        public Cell(
            string roomDefinitionId,
            CellType type,
            int x,
            int y,
            int z)
        {
            RoomDefinitionID = roomDefinitionId;
            Type = type;
            X = x;
            Y = y;
            Z = z;
        }

        #region Methods
        #endregion
    }
}
