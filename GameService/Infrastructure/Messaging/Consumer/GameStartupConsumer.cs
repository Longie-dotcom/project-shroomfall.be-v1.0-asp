using Application.Interface.Cache;
using Application.Service.WorldService;
using Contract.DTO.Messaging;
using MassTransit;

namespace Infrastructure.Messaging.Consumer
{
    public class GameStartupConsumer : IConsumer<GameStartupDTO>
    {
        #region Attributes
        private readonly ICacheProvider cacheProvider;
        private readonly BootstrapService bootstrapService;
        #endregion

        #region Properties
        #endregion

        public GameStartupConsumer(
            ICacheProvider cacheProvider,
            BootstrapService bootstrapService)
        {
            this.cacheProvider = cacheProvider;
            this.bootstrapService = bootstrapService;
        }

        #region Methods
        public async Task Consume(
            ConsumeContext<GameStartupDTO> context)
        {
            var message = context.Message;

            // LOAD CACHE
            await cacheProvider.LoadAllAsync(message.DefinitionCache);

            // BOOT WORLD
            await bootstrapService.LoadAsync();
        }
        #endregion
    }
}