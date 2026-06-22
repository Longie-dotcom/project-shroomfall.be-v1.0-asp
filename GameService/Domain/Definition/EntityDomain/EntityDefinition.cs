using Contract.Enum.EntityDomain;
using Domain.Definition.LocalizationDomain;

namespace Domain.Definition.EntityDomain
{
    public class EntityDefinition
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; private set; } = string.Empty;
        public EntityType Type { get; private set; }
        public EntityPresentationDefinition Presentation { get; private set; }
        #endregion

        protected EntityDefinition() { }

        public EntityDefinition(
            string id,
            EntityType type,
            EntityPresentationDefinition presentation)
        {
            ID = id;
            Type = type;
            Presentation = presentation;
        }

        #region Methods
        #endregion
    }

    public class EntityPresentationDefinition
    {
        #region Attributes
        #endregion

        #region Properties
        public LocalizedText LocalizedText { get; private set; } = new LocalizedText();
        public string? IconID { get; private set; } = string.Empty;
        #endregion

        protected EntityPresentationDefinition() { }

        public EntityPresentationDefinition(
            LocalizedText localizedText,
            string? iconId)
        {
            LocalizedText = localizedText;
            IconID = iconId;
        }

        #region Methods
        #endregion
    }
}