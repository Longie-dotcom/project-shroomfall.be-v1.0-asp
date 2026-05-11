namespace Application.Features.Abstraction
{
    public interface IHandler<TCommand, TResponse>
    {
        Task<TResponse> Handle(TCommand command);
    }

    public interface IHandler<TCommand>
    {
        Task Handle(TCommand command);
    }
}
