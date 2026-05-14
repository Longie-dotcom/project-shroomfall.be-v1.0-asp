using Application.Interfaces.Realtime;
using Application.Systems.Request;
using Application.Systems.Resolver;
using Application.Systems.Tick;
using Application.Systems.Trigger;
using Domain.Shared;
using Infrastructure.Realtime;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Background
{
    public class WorldLoopService : BackgroundService
    {
        #region Attributes
        private readonly MovementRequest movementRequest;

        private readonly CollisionResolver collisionResolver;

        private readonly MovementTrigger movementTrigger;

        private readonly EffectTick effectTickSystem;

        private readonly IEventBus eventBus;
        private readonly EventDispatcher dispatcher;

        private readonly List<CollisionRequest> collisionRequests;
        #endregion

        #region Properties
        #endregion

        public WorldLoopService(
            MovementRequest movementRequest,

            CollisionResolver collisionResolver,

            MovementTrigger movementTrigger,

            EffectTick effectTickSystem,

            IEventBus eventBus,
            EventDispatcher dispatcher)
        {
            this.movementRequest = movementRequest;

            this.collisionResolver = collisionResolver;

            this.movementTrigger = movementTrigger;

            this.effectTickSystem = effectTickSystem;

            this.eventBus = eventBus;
            this.dispatcher = dispatcher;

            collisionRequests = new List<CollisionRequest>();
        }

        #region Methods
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(Constraint.DELTA_TIME));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                // Request systems
                movementRequest.Update(Constraint.DELTA_TIME, collisionRequests);

                // Resolver systems 
                var collisionResults = collisionResolver.Resolve(collisionRequests);

                // Trigger systems
                movementTrigger.Apply(collisionResults);

                // UNKNOWN!!!
                effectTickSystem.Tick(Constraint.DELTA_TIME);

                // Clear requests
                collisionRequests.Clear();
                
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