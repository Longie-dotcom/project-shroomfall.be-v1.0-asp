using Domain.Definition.AttributeDomain.Enum;
using Domain.Definition.LocalizationDomain;
using Domain.DomainException;
using Domain.Shared;

namespace Domain.Definition.AttributeDomain
{
    public class Characteristic
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; private set; }
        public CharacteristicType Type { get; private set; }
        public LocalizedText LocalizedText { get; private set; }

        public ICollection<AttributeValue> AttributeValues { get; private set; } = new List<AttributeValue>();
        #endregion

        protected Characteristic()
        {

        }

        public Characteristic(
            string id,
            CharacteristicType type,
            LocalizedText localizedText)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new BadRequest(ResponseCode.Characteristic_InvalidId);

            if (string.IsNullOrWhiteSpace(localizedText.NameKey))
                throw new BadRequest(ResponseCode.Characteristic_InvalidName);

            if (string.IsNullOrWhiteSpace(localizedText.DescriptionKey))
                throw new BadRequest(ResponseCode.Characteristic_InvalidDescription);

            ID = id;
            Type = type;
            LocalizedText = localizedText;
        }

        #region Methods
        #endregion
    }
}
