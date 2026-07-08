using Domain.DomainException;
using ResponseCode;

namespace Domain.Definition.LocalizationDomain
{
    public class Locale
    {
        #region Attributes
        #endregion

        #region Properties
        public string Code { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public bool IsDefault { get; private set; }
        public bool IsEnabled { get; private set; }

        public List<LocalizationEntry> LocalizationEntries { get; private set; } = new();
        #endregion

        protected Locale() { }

        public Locale(
            string code,
            string name,
            bool isDefault = false,
            bool isEnabled = true)
        {
            Code = code.Trim().ToLowerInvariant();
            Name = name;
            IsDefault = isDefault;
            IsEnabled = isEnabled;
        }

        #region Methods
        public void Disable()
        {
            if (IsDefault)
                throw new BadRequest(
                    DomainCode.LocaleCode.CanNotDisableDefault,
                    "The default application locale cannot be disabled.");

            IsEnabled = false;
        }

        public void Enable()
        {
            IsEnabled = true;
        }
        #endregion
    }

    public class LocalizationEntry
    {
        #region Attributes
        #endregion

        #region Properties
        public Guid ID { get; private set; }
        public string Key { get; private set; } = string.Empty; // e.g. "item.wood_pickaxe.name"
        public string Value { get; private set; } = string.Empty; // localized text
        public string? Description { get; private set; } = string.Empty; // optional: for designer notes / tooltip context
        public int Version { get; private set; } // versioning for cache invalidation / updates
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; }

        public string LocaleCode { get; private set; } = string.Empty; // e.g. "en", "vi", "jp"
        public Locale Locale { get; private set; }
        #endregion

        protected LocalizationEntry() { }

        public LocalizationEntry(
            Guid id,
            string key,
            string localeCode,
            string? value,
            string? description = null)
        {
            ID = id;
            Key = key;
            LocaleCode = localeCode;
            Value = value ?? string.Empty;
            Description = description;

            Version = 1;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            IsDeleted = false;
        }

        #region Methods
        #endregion
    }

    public class LocalizedText
    {
        #region Attributes
        #endregion

        #region Properties
        public string NameKey { get; set; } = string.Empty;
        public string DescriptionKey { get; set; } = string.Empty;
        #endregion

        public LocalizedText() { }

        #region Methods
        #endregion
    }
}