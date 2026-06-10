using Application.Context;
using Application.Interfaces.Cache;
using Application.Interfaces.Factory;
using Contract.Enum.AttributeDomain;
using Domain.Definition.AttributeDomain;
using Domain.Definition.ItemDomain;
using Domain.Runtime.EntityDomain;
using Domain.Shared;

namespace Application.Services.AttributeService
{
    public class EffectService
    {
        #region Attributes
        private readonly IEffectInstanceFactory effectFactory;
        private readonly CharacteristicService characteristicService;
        private readonly IItemCache itemCache;
        private readonly IEffectCache effectCache;
        private readonly WorldContext worldContext;
        #endregion

        #region Properties
        #endregion

        public EffectService(
            IEffectInstanceFactory effectFactory,
            CharacteristicService characteristicService,
            IItemCache itemCache,
            IEffectCache effectCache,
            WorldContext worldContext)
        {
            this.effectFactory = effectFactory;
            this.characteristicService = characteristicService;
            this.itemCache = itemCache;
            this.effectCache = effectCache;
            this.worldContext = worldContext;
        }

        #region Methods
        // Active
        public void ApplyItemEffects(
            CreatureInstance creature,
            Item itemDef,
            string sourceItemInstanceId)
        {
            foreach (var itemEffect in itemDef.Effects)
            {
                var effect = effectFactory.Create(
                    itemEffect.EffectID,
                    sourceItemInstanceId);

                creature.ActiveEffects.Add(effect);
            }

            characteristicService.RecalculateCoreValues(creature);
        }

        public void RemoveItemEffects(
            CreatureInstance creature,
            string sourceItemInstanceId)
        {
            creature.ActiveEffects.RemoveAll(
                x => x.SourceItemInstanceID == sourceItemInstanceId);

            characteristicService.RecalculateCoreValues(creature);
        }

        // Passive / Combat Payload Execution Engine
        public void ExecuteInstantPayload(
            CreatureInstance target,
            string? sourceDefinitionId,
            string sourceEntityOwnerId)
        {
            var attacker = worldContext.GetEntity<CreatureInstance>(sourceEntityOwnerId);
            if (attacker == null) return;

            if (!CanDamage(attacker, target))
                return;

            // ─────────────────────────────────────────────────────────
            // CASE 1: Item-Driven Effects (Weapon/Spell/Projectile)
            // ─────────────────────────────────────────────────────────
            if (!string.IsNullOrEmpty(sourceDefinitionId))
            {
                var definition = itemCache.Get(sourceDefinitionId);
                if (definition == null) return;

                foreach (var effectRef in definition.Effects)
                {
                    var effectDef = effectCache.Get(effectRef.EffectID);
                    if (effectDef == null) continue;

                    ProcessEffectPayload(target, attacker, effectDef, sourceEntityOwnerId);
                }
            }
            // ─────────────────────────────────────────────────────────
            // CASE 2: Characteristic-Driven (Native Creature Attack)
            // ─────────────────────────────────────────────────────────
            else
            {
                // Run the raw monster attack through the mitigation resolver
                float finalDamage = CombatService.ResolveMitigatedDamage(attacker, target);

                // Apply health reduction
                characteristicService.ModifyVitalValue(target, AttributeType.Health, -finalDamage);
            }
        }

        /// <summary>
        /// Dedicated worker method that isolates the shared core/vital execution 
        /// pipeline for both items and creatures.
        /// </summary>
        private void ProcessEffectPayload(
            CreatureInstance target,
            CreatureInstance attacker,
            Effect effectDef,
            string sourceInstanceId)
        {
            var attrDef = AttributeDefinitions.Get(effectDef.AttributeType);
            if (attrDef == null) return;

            // Handle Duration-based status updates (Buffs, Debuffs, DoTs)
            if (effectDef.Duration.HasValue)
            {
                var liveEffect = effectFactory.Create(effectDef.ID, sourceInstanceId);
                target.ActiveEffects.Add(liveEffect);

                if (attrDef.DomainType == DomainType.Core)
                {
                    characteristicService.RecalculateCoreValues(target);
                }
                return;
            }

            // Handle direct, flat instant Vital adjustments (Damage, Healing)
            if (attrDef.DomainType == DomainType.Vital)
            {
                float finalPayloadDelta = effectDef.Value; // This is damage (effect on that same type) like: effect == Health meant this effect is modify health

                // Only perform combat resistance scaling if this effect is harmful (negative value)
                if (effectDef.Value < 0f)
                {
                    // Pass total raw package through Combat Service mitigation
                    float finalDamage = CombatService.ResolveMitigatedDamage(attacker, target);

                    // Re-apply negative orientation to target vital delta
                    finalPayloadDelta = -finalDamage;
                }

                // Mutate the final asset pool accurately
                characteristicService.ModifyVitalValue(
                    target,
                    effectDef.AttributeType,
                    finalPayloadDelta
                );
            }
        }

        private bool CanDamage(
            CreatureInstance attacker,
            CreatureInstance target)
        {
            bool attackerPlayer = attacker is PlayerInstance;
            bool targetPlayer = target is PlayerInstance;

            return attackerPlayer != targetPlayer;
        }
        #endregion
    }
}