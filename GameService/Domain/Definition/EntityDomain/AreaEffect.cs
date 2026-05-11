using Domain.Definition.EntityDomain.Component;
using Domain.Definition.EntityDomain.Enum;
using Domain.Definition.LocalizationDomain;

namespace Domain.Definition.EntityDomain
{
    public class AreaEffect : Entity
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        protected AreaEffect() 
        { 
        
        }

        public AreaEffect(
            string id,
            EntityType type,
            LocalizedText localizedText,
            Appearance appearance,
            Collision collision) : base(
                id,
                type,
                localizedText,
                appearance,
                collision)
        {

        }

        #region Methods
        #endregion
    }
}
