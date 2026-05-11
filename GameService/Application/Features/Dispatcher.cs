using Application.Features.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Features
{
    public class Dispatcher : IDispatcher
    {
        #region Attributes
        private readonly IServiceProvider provider;
        #endregion

        #region Properties
        #endregion

        public Dispatcher(
            IServiceProvider provider)
        {
            this.provider = provider;
        }

        #region Methods
        public async Task<TResponse> Send<TCommand, TResponse>(TCommand command)
        {
            var handler = provider.GetRequiredService<IHandler<TCommand, TResponse>>();
            return await handler.Handle(command);
        }

        public async Task Send<TCommand>(TCommand command)
        {
            var handler = provider.GetRequiredService<IHandler<TCommand>>();
            await handler.Handle(command);
        }
        #endregion
    }
}