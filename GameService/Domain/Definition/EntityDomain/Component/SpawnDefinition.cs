using Domain.Abstraction;
using Domain.Common;

namespace Domain.Definition.EntityDomain.Component
{
    public class SpawnDefinition : ComponentDefinition
    {
        #region Attributes
        #endregion

        #region Properties
        public List<SpawnEntry> SpawnEntries { get; private set; } = new();
        #endregion

        protected SpawnDefinition() : base() { }

        public SpawnDefinition(Guid id, string entityDefinitionId) : base(id, entityDefinitionId) { }

        #region Methods
        #endregion
    }

    public class SpawnEntry
    {
        #region Attributes
        #endregion

        #region Properties
        public Guid ID { get; set; }
        public string SpawnedEntityDefinitionID { get; private set; } = string.Empty;
        public Vector2 Offset { get; private set; } = Vector2.Zero;
        public int Count { get; private set; }

        public Guid SpawnDefinitionID { get; private set; }
        public SpawnDefinition SpawnDefinition { get; private set; }
        #endregion

        protected SpawnEntry() { }

        public SpawnEntry(
            Guid id,
            string spawnedEntityDefinitionId,
            Vector2 offset,
            int count,
            Guid spawnDefinitionId)
        {
            ID = id;
            SpawnedEntityDefinitionID = spawnedEntityDefinitionId;
            Offset = offset;
            Count = count;
            SpawnDefinitionID = spawnDefinitionId;
        }

        #region Methods
        #endregion
    }
}