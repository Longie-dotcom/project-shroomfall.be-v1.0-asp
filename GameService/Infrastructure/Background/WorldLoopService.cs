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

        private readonly ResidencyTick residencyTick;

        private readonly IEventBus eventBus;
        private readonly IEventDispatcher dispatcher;

        private readonly List<CreatureContext> creatureContexts;
        private readonly List<ProjectileContext> projectileContexts;
        private readonly List<string> projectileExpirations;
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

            ResidencyTick residencyTick,

            IEventBus eventBus,
            IEventDispatcher dispatcher)
        {
            this.creatureRequest = creatureRequest;
            this.creatureResolver = creatureResolver;
            this.creatureTrigger = creatureTrigger;

            this.projectileRequest = projectileRequest;
            this.projectileResolver = projectileResolver;
            this.projectileTrigger = projectileTrigger;

            this.residencyTick = residencyTick;

            this.eventBus = eventBus;
            this.dispatcher = dispatcher;

            creatureContexts = new List<CreatureContext>();
            projectileContexts = new List<ProjectileContext>();
            projectileExpirations = new List<string>();
        }

        #region Methods
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var timer = new PeriodicTimer(
                TimeSpan.FromSeconds(Constraint.DELTA_TIME));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                // Request systems
                creatureRequest.Update(Constraint.DELTA_TIME, creatureContexts);
                projectileRequest.Update(Constraint.DELTA_TIME, projectileContexts, projectileExpirations);

                // Resolver systems 
                var creatureResults = creatureResolver.Resolve(creatureContexts);
                var projectileResults = projectileResolver.Resolve(projectileContexts);

                // Trigger systems
                creatureTrigger.Apply(creatureResults);
                projectileTrigger.Apply(projectileResults, projectileExpirations);

                // Stats systems
                await residencyTick.Tick(Constraint.DELTA_TIME); 

                // Clear requests
                creatureContexts.Clear();
                
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