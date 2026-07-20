using Application.Interfaces.Realtime.Events;
using Application.Interfaces.Utility;
using Application.Services.WorldService;
using Application.Systems.Queue;
using Application.Systems.System;
using Contract;
using Domain.DomainException;
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
        private readonly ITelemetryQueue telemetryQueue;
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
            IEventDispatcher dispatcher,
            ITelemetryQueue telemetryQueue)
        {
            this.worldContext = worldContext;

            this.entityRequest = entityRequest;
            this.entityResolver = entityResolver;
            this.entityTrigger = entityTrigger;
            this.commandBuffer = commandBuffer;
            this.eventBus = eventBus;
            this.dispatcher = dispatcher;
            this.telemetryQueue = telemetryQueue;
        }

        #region Methods
        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            var timer = new PeriodicTimer(
                TimeSpan.FromSeconds(Constraint.DELTA_TIME));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
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
                catch (Exception ex)
                {
                    switch (ex)
                    {
                        case BadRequest badRequest:
                            telemetryQueue.EnqueueAlert(
                                badRequest.Code,
                                badRequest.Message,
                                TelemetrySeverity.Error);
                            break;

                        case NotFound notFound:
                            telemetryQueue.EnqueueAlert(
                                notFound.Code,
                                notFound.Message,
                                TelemetrySeverity.Error);
                            break;

                        case Unauthorized unauthorized:
                            telemetryQueue.EnqueueAlert(
                                unauthorized.Code,
                                unauthorized.Message,
                                TelemetrySeverity.Error);
                            break;

                        case InternalException internalException:
                            telemetryQueue.EnqueueAlert(
                                internalException.Code,
                                internalException.Message,
                                TelemetrySeverity.Fatal);
                            break;

                        default:
                            telemetryQueue.EnqueueAlert(
                                "world.loop.unhandled",
                                ex.Message,
                                TelemetrySeverity.Fatal);

                            Console.WriteLine(ex.ToString());
                            break;
                        }
                    }
                }
            }
        #endregion
    }
}