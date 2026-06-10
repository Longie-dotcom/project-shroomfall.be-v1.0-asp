using Contract.Enum.EntityDomain;
using Domain.Definition.EntityDomain.Component;
using Domain.Definition.LocalizationDomain;
using Domain.DomainException;
using Domain.Shared;

namespace Domain.Definition.EntityDomain
{
    public class AreaEffect : Entity
    {
        #region Attributes
        #endregion

        #region Properties
        public float Duration { get; private set; }
        #endregion

        protected AreaEffect() 
        { 
        
        }

        public AreaEffect(
            string id,
            EntityType type,
            LocalizedText localizedText,
            Appearance appearance,
            Collision collision,
            float duration) : base(
                id,
                type,
                localizedText,
                appearance,
                collision)
        {
            if (duration <= 0f)
                throw new InternalException(
                    ResponseCode.AreaEffectDefinition_InvalidDuration,
                    $"Area effect blueprint '{id}' cannot be initialized with a duration of {duration}. Lifetime duration must be greater than zero.");

            Duration = duration;
        }

        #region Methods
        #endregion
    }
}
