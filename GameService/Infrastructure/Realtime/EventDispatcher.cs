using Application.Events.Abstraction;
using Application.Interfaces.Realtime;

namespace Infrastructure.Realtime
{
    public class EventDispatcher : IEventDispatcher
    {
        #region Attributes
        private readonly IEnumerable<IEventHandler> handlers;
        #endregion

        #region Properties
        #endregion

        public EventDispatcher(
            IEnumerable<IEventHandler> handlers)
        {
            this.handlers = handlers;
        }

        #region Methods
        public async Task Dispatch(
            IEvent @event)
        {
            foreach (var handler in handlers)
            {
                await handler.Handle(@event);
            }
        }
        #endregion
    }
}