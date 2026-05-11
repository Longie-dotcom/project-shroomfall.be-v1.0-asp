using Domain.Definition.AttributeDomain.Enum;
using Domain.Definition.LocalizationDomain;
using Domain.Shared;

namespace Domain.Definition.AttributeDomain
{
    public class AttributeDefinition
    {
        #region Attributes
        #endregion

        #region Properties
        public AttributeType Type { get; set; }
        public LocalizedText LocalizedText { get; set; } = new LocalizedText();
        public ValueCategory Category { get; set; }
        public DomainType DomainType { get; set; }
        #endregion

        // Note: These objects are stored by on-memory database: AttributeDefinitions

        public AttributeDefinition()
        {

        }

        #region Methods
        #endregion
    }
}