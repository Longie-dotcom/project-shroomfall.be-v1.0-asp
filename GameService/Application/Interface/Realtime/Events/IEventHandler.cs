namespace Application.Interface.Realtime.Events
{
    public interface IEventHandler
    {
        Task Handle(
            IEvent @event);
    }
}
