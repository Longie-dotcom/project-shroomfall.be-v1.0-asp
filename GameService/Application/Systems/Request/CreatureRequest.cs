using Application.Context;
using Application.Interfaces.Cache;
using Application.Services.AttributeService;
using Application.Services.EntityService;
using Application.Services.WorldService;
using Application.Systems.Resolver;
using Contract.Enum.AttributeDomain;
using Domain.Runtime.EntityDomain;
using Domain.Shared;

namespace Application.Systems.Request
{
    public class CreatureRequest
    {
        #region Attributes
        private readonly WorldContext worldContext;
        private readonly CharacteristicService characteristicService;
        private readonly IEffectCache effectCache;
        private readonly CreatureAIService creatureAIService;
        #endregion

        #region Properties
        #endregion

        public CreatureRequest(
            WorldContext worldContext,
            CharacteristicService characteristicService,
            IEffectCache effectCache,
            CreatureAIService creatureAIService)
        {
            this.worldContext = worldContext;
            this.characteristicService = characteristicService;
            this.effectCache = effectCache;
            this.creatureAIService = creatureAIService;
        }

        #region Methods
        public List<CreatureContext> Update(float dt)
        {
            // 1. Create a fresh list for the current frame
            var results = new List<CreatureContext>();

            // 2. Take a snapshot to prevent "Collection was modified" exceptions
            var creatures = worldContext.GetEntities<CreatureInstance>().ToList();

            foreach (var creature in creatures)
            {
                creatureAIService.TickAI(dt, creature);
                TickEffect(dt, creature);

                // 1. Assign to a local variable
                var context = CreateMovementContext(dt, creature);

                // 2. Check the local variable
                if (context != null)
                {
                    results.Add((CreatureContext)context);
                }
            }

            return results;
        }

        private CreatureContext? CreateMovementContext(float dt, CreatureInstance creature)
        {
            if (!creature.WantsToMove)
                return null;

            float speed = creature.Characteristic.GetCore(AttributeType.MoveSpeed);
            var desired = creature.Position + creature.MovementVector * speed * dt;

            var body = new CollisionBody(
                creature.ID,
                creature.RoomSpatialID,
                creature.Position,
                creature.CollisionOffset,
                creature.LayerZ,
                creature.CollisionShape);

            return new CreatureContext(creature.ID, body, desired);
        }

        private void TickEffect(
            float dt,
            CreatureInstance creature)
        {
            bool coreValuesChanged = false;

            for (int i = creature.ActiveEffects.Count - 1; i >= 0; i--)
            {
                var effect = creature.ActiveEffects[i];

                // Advances both RemainingTime and IntervalAccumulator via your method
                effect.Tick(dt);

                var effectDef = effectCache.Get(effect.DefinitionID);
                if (effectDef == null) continue;

                var attrDef = AttributeDefinitions.Get(effectDef.AttributeType);
                if (attrDef == null) continue;

                //  Processes interval consumption for Regen/DoT effects
                if (effectDef.Interval.HasValue)
                {
                    if (effect.TryConsumeInterval(effectDef.Interval.Value))
                    {
                        float value = effectDef.Value;

                        if (value < 0) // damage effect
                        {
                            value = -CombatService.ResolveMitigatedDamage(
                                creature,
                                Math.Abs(value),
                                effectDef.AttributeType);
                        }

                        characteristicService.ModifyVitalValue(
                            creature,
                            effectDef.AttributeType,
                            value);
                    }
                }

                // Prunes expired instances using your IsExpired() validation
                if (effect.IsExpired())
                {
                    creature.ActiveEffects.RemoveAt(i);

                    if (attrDef.DomainType == DomainType.Core)
                    {
                        coreValuesChanged = true;
                    }
                }
            }

            if (coreValuesChanged)
            {
                characteristicService.RecalculateCoreValues(creature);
            }
        }

        private void TickMovement(
            float dt,
            CreatureInstance creature,
            List<CreatureContext> contexts)
        {
            if (!creature.WantsToMove)
                return;

            // Resolve creature desired position
            float speed = creature.Characteristic.GetCore(AttributeType.MoveSpeed);
            var desired = creature.Position + creature.MovementVector * speed * dt;

            // Request for collision resolving
            var body = new CollisionBody(
                creature.ID,
                creature.RoomSpatialID,
                creature.Position,
                creature.CollisionOffset,
                creature.LayerZ,
                creature.CollisionShape);

            contexts.Add(new CreatureContext(
                creature.ID,
                body,
                desired));
        }
        #endregion
    }
}