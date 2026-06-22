using Domain.Abstraction;

namespace Domain.Runtime.EntityDomain.Component
{
    public class LifetimeInstance : ComponentInstance
    {
        #region Attributes
        #endregion

        #region Properties
        public float Duration { get; }
        public float ElapsedLifetime { get; private set; }
        #endregion

        public LifetimeInstance(
            Guid definitionId,
            float duration,
            float elapsedLifetime = 0f) : base(definitionId)
        {
            Duration = duration;
            ElapsedLifetime = elapsedLifetime;
        }

        #region Methods
        public void TickLifetime(
            float dt)
        {
            ElapsedLifetime += dt;
        }

        public bool IsExpired()
        {
            return ElapsedLifetime >= Duration;
        }
        #endregion
    }
}