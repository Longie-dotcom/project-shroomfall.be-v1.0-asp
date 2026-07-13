using Application.Interfaces.Realtime.Events;
using Application.Services.WorldService;
using Application.Systems.Queue;
using Application.Systems.System;
using Contract;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Background
{
    public class WorldLoopService : BackgroundService
    {
        #region Attributes
        private readonly WorldContext worldContext;

        private readonly EntityRequest entityRequest;
        private readonly EntityResolver entityResolver;
        private readonly EntityTrigger entityTrigger;

        private readonly CommandBuffer commandBuffer;

        private readonly IEventBus eventBus;
        private readonly IEventDispatcher dispatcher;
        #endregion

        #region Properties
        #endregion

        public WorldLoopService(
            WorldContext worldContext,

            EntityRequest entityRequest,
            EntityResolver entityResolver,
            EntityTrigger entityTrigger,

            CommandBuffer commandBuffer,

            IEventBus eventBus,
            IEventDispatcher dispatcher)
        {
            this.worldContext = worldContext;

            this.entityRequest = entityRequest;
            this.entityResolver = entityResolver;
            this.entityTrigger = entityTrigger;

            this.commandBuffer = commandBuffer;

            this.eventBus = eventBus;
            this.dispatcher = dispatcher;
        }

        #region Methods
        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            var timer = new PeriodicTimer(
                TimeSpan.FromSeconds(Constraint.DELTA_TIME));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                // Ticking
                worldContext.AdvanceTick();

                // Systems
                await entityRequest.Tick(Constraint.DELTA_TIME, commandBuffer);
                entityResolver.Resolve(commandBuffer);
                entityTrigger.Apply(commandBuffer);

                // Drain events
                var events = eventBus.Drain();

                // Publish realtime
                foreach (var e in events)
                {
                    await dispatcher.Dispatch(e);
                }
            }
        }
        #endregion
    }
}