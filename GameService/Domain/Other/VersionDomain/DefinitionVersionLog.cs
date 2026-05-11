using Domain.DomainException;
using Domain.Shared;

namespace Domain.Other.VersionDomain
{
    public class DefinitionVersionLog
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; private set; }
        public string Key { get; private set; } // Future cache loading optimize
        public long Version { get; private set; }
        public string? Description { get; private set; }
        public DateTime CreatedAt { get; private set; }
        #endregion

        protected DefinitionVersionLog() 
        {
        
        }

        public DefinitionVersionLog(
            string id,
            string key,
            long version,
            string? description)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new BadRequest(ResponseCode.DefinitionVersionLog_InvalidId);

            if (string.IsNullOrWhiteSpace(key))
                throw new BadRequest(ResponseCode.DefinitionVersionLog_InvalidKey);

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