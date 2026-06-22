namespace Domain.Definition
{
    public class DefinitionVersionLog
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; private set; } = string.Empty;
        public string Key { get; private set; } = string.Empty;
        public long Version { get; private set; }
        public string? Description { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }
        #endregion

        protected DefinitionVersionLog() { }

        public DefinitionVersionLog(
            string id,
            string key,
            long version,
            string? description)
        {
            ID = id;
            Key = key;
            Version = version;
            Description = description;
            CreatedAt = DateTime.Now;
        }

        #region Methods
        #endregion
    }
}