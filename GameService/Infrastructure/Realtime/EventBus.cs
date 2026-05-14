using Application.Events.Abstraction;
using Application.Interfaces.Realtime;

namespace Infrastructure.Realtime
{
    public class EventBus : IEventBus
    {
        #region Attributes
        private List<IEvent> current = new(1024);
        private List<IEvent> next = new(1024);
        private readonly object _lock = new();
        #endregion

        #region Properties
        #endregion

        public EventBus()
        {

        }

        #region Methods
        public void Publish(
            IEvent @event)
        {
            lock (_lock)
            {
                next.Add(@event);
            }
        }

        public List<IEvent> Drain()
        {
            lock (_lock)
            {
                var result = current;

                current = next;
                next = new List<IEvent>(1024);

                return result;
            }
        }
        #endregion
    }
}