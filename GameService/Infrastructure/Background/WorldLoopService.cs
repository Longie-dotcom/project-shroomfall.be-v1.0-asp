using Application.Interfaces.Realtime;
using Application.Systems.Request;
using Application.Systems.Resolver;
using Application.Systems.Tick;
using Application.Systems.Trigger;
using Contract;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Background
{
    public class WorldLoopService : BackgroundService
    {
        #region Attributes
        private readonly CreatureRequest creatureRequest;
        private readonly CreatureResolver creatureResolver;
        private readonly CreatureTrigger creatureTrigger;

        private readonly ProjectileRequest projectileRequest;
        private readonly ProjectileResolver projectileResolver;
        private readonly ProjectileTrigger projectileTrigger;

        private readonly AreaEffectRequest areaEffectRequest;
        private readonly AreaEffectResolver areaEffectResolver;
        private readonly AreaEffectTrigger areaEffectTrigger;

        private readonly ResidencyTick residencyTick;

        private readonly IEventBus eventBus;
        private readonly IEventDispatcher dispatcher;
        #endregion

        #region Properties
        #endregion

        public WorldLoopService(
            CreatureRequest creatureRequest,
            CreatureResolver creatureResolver,
            CreatureTrigger creatureTrigger,

            ProjectileRequest projectileRequest,
            ProjectileResolver projectileResolver,
            ProjectileTrigger projectileTrigger,

            AreaEffectRequest areaEffectRequest,
            AreaEffectResolver areaEffectResolver,
            AreaEffectTrigger areaEffectTrigger,

            ResidencyTick residencyTick,

            IEventBus eventBus,
            IEventDispatcher dispatcher)
        {
            this.creatureRequest = creatureRequest;
            this.creatureResolver = creatureResolver;
            this.creatureTrigger = creatureTrigger;

            this.areaEffectRequest = areaEffectRequest;
            this.areaEffectResolver = areaEffectResolver;
            this.areaEffectTrigger = areaEffectTrigger;

            this.projectileRequest = projectileRequest;
            this.projectileResolver = projectileResolver;
            this.projectileTrigger = projectileTrigger;

            this.residencyTick = residencyTick;

            this.eventBus = eventBus;
            this.dispatcher = dispatcher;
        }

        #region Methods
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var timer = new PeriodicTimer(
                TimeSpan.FromSeconds(Constraint.DELTA_TIME));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                // Request systems
                var creatureContexts = creatureRequest.Update(Constraint.DELTA_TIME);
                var (projectileContexts, projectileExpirations) = projectileRequest.Update(Constraint.DELTA_TIME);
                var (areaEffectContexts, areaEffectExpirations) = areaEffectRequest.Update(Constraint.DELTA_TIME);

                // Resolver systems 
                var creatureResults = creatureResolver.Resolve(creatureContexts);
                var projectileResults = projectileResolver.Resolve(projectileContexts);
                var areaEffectResults = areaEffectResolver.Resolve(areaEffectContexts);

                // Trigger systems
                creatureTrigger.Apply(creatureResults);
                projectileTrigger.Apply(projectileResults, projectileExpirations);
                areaEffectTrigger.Apply(areaEffectResults, areaEffectExpirations);

                // Stats systems
                await residencyTick.Tick(Constraint.DELTA_TIME); 
                
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