namespace Application.Feature.Abstraction
{
    public interface IDispatcher
    {
        Task<TResponse> Send<TCommand, TResponse>(
            TCommand command);
        Task Send<TCommand>(
            TCommand command);
    }
}
