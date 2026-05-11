using Domain.DomainException;
using Domain.Shared;

namespace Domain.Definition.LocalizationDomain
{
    public class Locale
    {
        #region Attributes
        #endregion

        #region Properties
        public string Code { get; private set; }
        public string Name { get; private set; }
        public bool IsDefault { get; private set; }
        public bool IsEnabled { get; private set; }

        public ICollection<LocalizationEntry> LocalizationEntries { get; private set; }
        #endregion

        protected Locale() 
        { 
        
        }

        public Locale(
            string code,
            string name,
            bool isDefault = false,
            bool isEnabled = true)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new BadRequest(ResponseCode.Locale_InvalidCode);

            if (string.IsNullOrWhiteSpace(name))
                throw new BadRequest(ResponseCode.Locale_InvalidName);

            Code = code.Trim().ToLowerInvariant();
            Name = name;
            IsDefault = isDefault;
            IsEnabled = isEnabled;
        }

        #region Methods
        public void Disable()
        {
            if (IsDefault)
                throw new BadRequest(ResponseCode.Locale_CanNotDisableDefault);

            IsEnabled = false;
        }

        public void Enable()
        {
            IsEnabled = true;
        }
        #endregion
    }
}