using Contract.Enum.EntityDomain;
using Domain.Definition.EntityDomain.Component;
using Domain.Definition.LocalizationDomain;

namespace Domain.Definition.EntityDomain
{
    public class Projectile : Entity
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        protected Projectile()
        {

        }

        public Projectile(
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
