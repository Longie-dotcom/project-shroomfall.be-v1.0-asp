namespace Application.Interfaces.Realtime.Events
{
    public interface IEventHandler
    {
        Task Handle(
            IEvent @event);
    }
}
