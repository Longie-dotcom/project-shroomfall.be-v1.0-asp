using Application.Context;
using Application.Interfaces.Cache;
using Application.Services.AttributeService;
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
        #endregion

        #region Properties
        #endregion

        public CreatureRequest(
            WorldContext worldContext,
            CharacteristicService characteristicService,
            IEffectCache effectCache)
        {
            this.worldContext = worldContext;
            this.characteristicService = characteristicService;
            this.effectCache = effectCache;
        }

        #region Methods
        public void Update(
            float dt,
            List<CreatureContext> contexts)
        {
            foreach (var creature in worldContext.GetEntities<CreatureInstance>())
            {
                TickEffect(dt, creature);

                TickMovement(dt, creature, contexts);
            }
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
                    // Uses your exact method name and tracks perfectly
                    if (effect.TryConsumeInterval(effectDef.Interval.Value))
                    {
                        characteristicService.ModifyVitalValue(creature, effectDef.AttributeType, effectDef.Value);
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