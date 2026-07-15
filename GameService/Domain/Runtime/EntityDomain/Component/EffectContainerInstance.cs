using Domain.Abstraction;
using Domain.Runtime.MetaDomain;

namespace Domain.Runtime.EntityDomain.Component
{
    public class EffectContainerInstance : ComponentInstance
    {
        #region Attributes
        #endregion

        #region Properties
        public List<EffectInstance> TrackingEffects { get; } = new List<EffectInstance>();
        #endregion

        public EffectContainerInstance() : base(Guid.Empty)
        {
            TrackingEffects = new List<EffectInstance>();
        }

        #region Methods
        #endregion
    }
}