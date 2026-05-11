using Domain.DomainException;
using Domain.Shared;

namespace Domain.Definition.LocalizationDomain
{
    public class LocalizationEntry
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; private set; }
        public string Key { get; private set; } // e.g. "item.wood_pickaxe.name"
        public string LocaleCode { get; private set; } // e.g. "en", "vi", "jp"
        public string Value { get; private set; } // localized text
        public string? Description { get; private set; } // optional: for designer notes / tooltip context
        public int Version { get; private set; } // versioning for cache invalidation / updates
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; }

        public Locale Locale { get; private set; }
        #endregion

        protected LocalizationEntry() 
        { 
        
        }

        public LocalizationEntry(
            string id,
            string key,
            string localeCode,
            string value,
            string? description = null)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new BadRequest(ResponseCode.LocalizationEntry_InvalidId);

            if (string.IsNullOrWhiteSpace(key))
                throw new BadRequest(ResponseCode.LocalizationEntry_InvalidKey);

            if (string.IsNullOrWhiteSpace(localeCode))
                throw new BadRequest(ResponseCode.LocalizationEntry_InvalidLocaleCode);

            if (string.IsNullOrWhiteSpace(value))
                throw new BadRequest(ResponseCode.LocalizationEntry_InvalidValue);

            ID = id;
            Key = key;
            LocaleCode = localeCode;
            Value = value;
            Description = description;

            Version = 1;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            IsDeleted = false;
        }

        #region Methods
        #endregion
    }
}