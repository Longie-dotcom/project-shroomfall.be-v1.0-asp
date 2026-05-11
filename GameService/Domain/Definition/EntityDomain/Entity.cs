using Domain.Definition.EntityDomain.Component;
using Domain.Definition.EntityDomain.Enum;
using Domain.Definition.LocalizationDomain;
using Domain.DomainException;
using Domain.Shared;

namespace Domain.Definition.EntityDomain
{
    public class Entity
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; private set; }
        public EntityType Type { get; private set; }
        public LocalizedText LocalizedText { get; private set; }
        public Appearance Appearance { get; private set; }
        public Collision Collision { get; private set; }
        #endregion

        protected Entity() 
        { 
        
        }

        public Entity(
            string id,
            EntityType type,
            LocalizedText localizedText,
            Appearance appearance,
            Collision collision)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new BadRequest(ResponseCode.Entity_InvalidId);

            if (string.IsNullOrWhiteSpace(localizedText.NameKey))
                throw new BadRequest(ResponseCode.Entity_InvalidName);

            if (string.IsNullOrWhiteSpace(localizedText.DescriptionKey))
                throw new BadRequest(ResponseCode.Entity_InvalidDescription);

            if (string.IsNullOrWhiteSpace(appearance.SkinID))
                throw new BadRequest(ResponseCode.Entity_InvalidSkinId);

            ID = id;
            Type = type;
            LocalizedText = localizedText;
            Appearance = appearance;
            Collision = collision;
        }

        #region Methods
        #endregion
    }
}
