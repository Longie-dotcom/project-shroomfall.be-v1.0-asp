using Contract.Enum.EntityDomain;
using Domain.Definition.EntityDomain.Component;
using Domain.Definition.LocalizationDomain;
using Domain.DomainException;
using Domain.Shared;

namespace Domain.Definition.EntityDomain
{
    public class Projectile : Entity
    {
        #region Attributes
        #endregion

        #region Properties
        public float Velocity { get; private set; }
        public float Duration { get; private set; }
        #endregion

        protected Projectile()
        {

        }

        public Projectile(
            string id, 
            EntityType type, 
            LocalizedText localizedText,
            Appearance appearance, 
            Collision collision,
            float velocity,
            float duration) : base(
                id, 
                type,
                localizedText,
                appearance, 
                collision)
        {
            if (velocity <= 0f)
                throw new InternalException(
                    ResponseCode.ProjectileDefinition_InvalidVelocity,
                    $"Projectile blueprint '{id}' cannot be initialized with a velocity of {velocity}. Speed must be greater than zero.");

            if (duration <= 0f)
                throw new InternalException(
                    ResponseCode.ProjectileDefinition_InvalidDuration,
                    $"Projectile blueprint '{id}' cannot be initialized with a duration of {duration}. Lifetime duration must be greater than zero.");

            Velocity = velocity;
            Duration = duration;
        }

        #region Methods
        #endregion
    }
}
