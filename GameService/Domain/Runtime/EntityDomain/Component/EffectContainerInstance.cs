using Domain.Abstraction;
using Domain.Runtime.MetaDomain;

namespace Domain.Runtime.EntityDomain.Component
{
    public class EffectContainerInstance : ComponentInstance
    {
        #region Attributes
        #endregion

        #region Properties
        public Dictionary<string, EffectInstance> TemporaryEffects { get; private set; }
        public Dictionary<string, EffectInstance> PermanentEffects { get; private set; }
        #endregion

        public EffectContainerInstance() : base(Guid.Empty)
        {
            TemporaryEffects = new Dictionary<string, EffectInstance>();
            PermanentEffects = new Dictionary<string, EffectInstance>();
        }

        #region Methods
        public IEnumerable<EffectInstance> GetAllPersistentEffects()
        {
            return TemporaryEffects.Values.Concat(PermanentEffects.Values);
        }
        #endregion
    }
}