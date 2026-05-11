using Application.Events.Abstraction;
using Application.Systems;
using Application.Systems.Resolver;
using Application.Systems.System;
using Application.Systems.Trigger;
using Domain.Shared;
using Infrastructure.Realtime;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Background
{
    public class WorldLoopService : BackgroundService
    {
        #region Attributes
        private readonly MovementSystem movementSystem;
        private readonly MovementTrigger movementTrigger;
        private readonly CollisionResolver collisionSystem;
        private readonly EntityLifecycleSystem entityLifecycleSystem;
        private readonly RoomTransitionSystem roomTransitionSystem;
        private readonly EffectTickSystem effectTickSystem;

        private readonly IEventBus eventBus;
        private readonly EventDispatcher dispatcher;

        private readonly List<CollisionRequest> collisionRequests;
        #endregion

        #region Properties
        #endregion

        public WorldLoopService(
            MovementSystem movementSystem,
            MovementTrigger movementTrigger,
            CollisionResolver collisionSystem,
            EntityLifecycleSystem entityLifecycleSystem,
            RoomTransitionSystem roomTransitionSystem,
            EffectTickSystem effectTickSystem,

            IEventBus eventBus,
            EventDispatcher dispatcher)
        {
            this.movementSystem = movementSystem;
            this.movementTrigger = movementTrigger;
            this.collisionSystem = collisionSystem;
            this.entityLifecycleSystem = entityLifecycleSystem;
            this.roomTransitionSystem = roomTransitionSystem;
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
                movementSystem.Update(Constraint.DELTA_TIME, collisionRequests);

                // Resolve systems 
                effectTickSystem.Update(Constraint.DELTA_TIME);
                var collisionResults = collisionSystem.ResolveBatch(collisionRequests);

                // Trigger systems
                movementTrigger.Apply(collisionResults);
                entityLifecycleSystem.Update();
                roomTransitionSystem.Update();

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