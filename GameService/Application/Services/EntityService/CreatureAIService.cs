using Application.Context;
using Application.Coordinator;
using Application.Interfaces.Cache;
using Application.Services.WorldService;
using Contract.Enum.AttributeDomain;
using Contract.Enum.EntityDomain;
using Domain.Common;
using Domain.Definition.EntityDomain;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;

namespace Application.Services.EntityService
{
    public class CreatureAIService
    {
        #region Attributes
        private readonly WorldContext worldContext;
        private readonly CollisionService collisionService;
        private readonly IEntityRelationshipCache entityRelationshipCache;
        private readonly EntityLifeCycleCoordinator entityLifeCycleCoordinator;
        #endregion

        #region Properties
        #endregion

        public CreatureAIService(
            WorldContext worldContext,
            CollisionService collisionService,
            IEntityRelationshipCache entityRelationshipCache,
            EntityLifeCycleCoordinator entityLifeCycleCoordinator)
        {
            this.worldContext = worldContext;
            this.collisionService = collisionService;
            this.entityRelationshipCache = entityRelationshipCache;
            this.entityLifeCycleCoordinator = entityLifeCycleCoordinator;
        }

        #region Methods
        public void TickAI(
            float dt,
            CreatureInstance creature)
        {
            creature.ThinkCooldownRemaining -= dt;

            if (creature.ThinkCooldownRemaining > 0)
                return;

            creature.ThinkCooldownRemaining = 0.25f;

            switch (creature.AIState)
            {
                case AIState.Idle:
                    TickIdle(creature);
                    break;

                case AIState.Wander:
                    TickWander(creature);
                    break;

                case AIState.Chase:
                    TickChase(creature);
                    break;

                case AIState.Attack:
                    TickAttack(dt, creature);
                    break;
            }
        }

        private void TickIdle(
            CreatureInstance creature)
        {
            var body = new CollisionBody(
                creature.ID,
                creature.RoomSpatialID,
                creature.Position,
                creature.CollisionOffset,
                creature.LayerZ,
                new CircleShape(5f, false, false)); // aggro radius

            var collision =
                collisionService.QueryOverlap(
                    body,
                    creature.Position);

            foreach (var entity in collision.Entities)
            {
                if (entity is PlayerInstance player)
                {
                    creature.TargetEntityId = player.ID;
                    creature.AIState = AIState.Chase;
                    return;
                }
            }

            creature.AIState = AIState.Wander;
        }

        private void TickWander(
            CreatureInstance creature)
        {
            Vector2 randomDirection = Vector2.Normalize(
                new Vector2(
                    Random.Shared.NextSingle() - 0.5f,
                    Random.Shared.NextSingle() - 0.5f));

            creature.SetMovementIntent(randomDirection);
        }

        private void TickChase(
            CreatureInstance creature)
        {
            if (string.IsNullOrEmpty(creature.TargetEntityId))
            {
                creature.AIState = AIState.ReturnHome;
                return;
            }

            var target = worldContext.GetEntity<PlayerInstance>(
                creature.TargetEntityId);

            if (target == null)
            {
                creature.AIState = AIState.ReturnHome;
                return;
            }

            float distance = Vector2.Distance(
                creature.Position,
                target.Position);

            if (distance > creature.LeashDistance)
            {
                creature.TargetEntityId = null;
                creature.AIState = AIState.ReturnHome;
                return;
            }

            if (distance <= 1.5f)
            {
                creature.AIState = AIState.Attack;
                return;
            }

            creature.SetMovementIntent(Vector2.Normalize(
                target.Position - creature.Position));
        }

        private void TickAttack(
            float dt,
            CreatureInstance creature)
        {
            creature.AttackTimer -= dt;

            if (creature.AttackTimer > 0)
                return;

            if (string.IsNullOrEmpty(creature.TargetEntityId))
            {
                creature.AIState = AIState.ReturnHome;
                return;
            }

            var target =
                worldContext.GetEntity<CreatureInstance>(
                    creature.TargetEntityId);

            if (target == null)
            {
                creature.AIState = AIState.ReturnHome;
                return;
            }

            var attacks =
                entityRelationshipCache.GetBySourceID(creature.DefinitionID);

            var attack = attacks?.FirstOrDefault();
            if (attack == null)
                return;

            ExecuteAttack(creature, target, attack);

            float attackSpeed =
                creature.Characteristic.GetCore(AttributeType.AttackSpeed);

            float interval = attackSpeed <= 0f
                ? 1f
                : 1f / attackSpeed;

            creature.AttackTimer = interval;
        }

        private void ExecuteAttack(
            CreatureInstance attacker,
            CreatureInstance target,
            EntityRelationship attack)
        {
            Vector2 direction = Vector2.Normalize(target.Position - attacker.Position);

            switch (attack.Type)
            {
                case EntityRelationshipType.ProjectileTriggeredBy:
                    SpawnProjectileAttack(attacker, attack, direction);
                    break;

                case EntityRelationshipType.AreaEffectTriggeredBy:
                    SpawnAreaAttack(attacker, attack, direction);
                    break;
            }
        }

        private void SpawnProjectileAttack(
            CreatureInstance attacker,
            EntityRelationship attack,
            Vector2 direction)
        {
            entityLifeCycleCoordinator.SpawnProjectile(
                projectileDefinitionId: attack.TargetEntityID,
                roomSpatialId: attacker.RoomSpatialID,
                layerZ: attacker.LayerZ,
                position: attacker.Position,
                direction: direction,
                ownerId: attacker.ID,
                sourceDefinitionId: attack.SourceEntityID
            );
        }

        private void SpawnAreaAttack(
            CreatureInstance attacker,
            EntityRelationship attack,
            Vector2 direction)
        {
            entityLifeCycleCoordinator.SpawnAreaEffect(
                areaEffectDefinitionId: attack.TargetEntityID,
                roomSpatialId: attacker.RoomSpatialID,
                layerZ: attacker.LayerZ,
                position: attacker.Position,
                ownerId: attacker.ID,
                sourceDefinitionId: attack.SourceEntityID
            );
        }
        #endregion
    }
}