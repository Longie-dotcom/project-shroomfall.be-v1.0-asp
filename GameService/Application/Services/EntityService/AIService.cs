using Application.Services.WorldService;
using Application.Systems.Abstraction;
using Application.Systems.Queue;
using Contract.Enum.EntityDomain;
using Contract.Enum.MetaDomain.Effect;
using Contract.Enum.MetaDomain.Item;
using Domain.Common;
using Domain.Definition.EntityDomain.Component;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;

namespace Application.Services.EntityService
{
    public class AIService : ITickService
    {
        #region Attributes
        private readonly WorldContext worldContext;
        private readonly CollisionService collisionService;
        #endregion

        public AIService(
            WorldContext worldContext,
            CollisionService collisionService)
        {
            this.worldContext = worldContext;
            this.collisionService = collisionService;
        }

        #region Methods
        public void Tick(
            float dt,
            CommandBuffer commandBuffer)
        {
            var entities = worldContext.GetEntities().ToList();

            foreach (var entity in entities)
            {
                var ai = entity.GetComponent<AIInstance>();
                if (ai == null || !ai.IsAIControlled)
                    continue;

                ai.ThinkCooldownRemaining -= dt;
                if (ai.ThinkCooldownRemaining > 0)
                    continue;

                ai.ThinkCooldownRemaining = 0.25f;

                switch (ai.AIState)
                {
                    case AIState.Idle: TickIdle(entity, ai); break;
                    case AIState.Wander: TickWander(entity); break;
                    case AIState.Chase: TickChase(entity, ai); break;
                    case AIState.Attack: TickAttack(dt, entity, ai); break;
                }
            }
        }

        private void TickIdle(
            EntityInstance entity,
            AIInstance ai)
        {
            var transform = entity.GetComponent<TransformInstance>();
            if (transform == null) return;

            var detectionShape = new CircleShape(ai.AggroRadius, isBlocking: false);

            var body = new CollisionBody(
                entity.ID,
                transform.RoomSpatialID,
                transform.Position,
                new Vector2(0, 0),
                transform.LayerZ,
                detectionShape,
                CollisionLayer.None,
                CollisionLayer.Player); // Follow player only

            var collision = collisionService.QueryOverlap(body, transform.Position);
            
            foreach (var hit in collision.Entities)
            {
                ai.TargetEntityId = hit.ID;
                ai.AIState = AIState.Chase;
                return;
            }

            ai.AIState = AIState.Wander;
        }

        private void TickWander(
            EntityInstance entity)
        {
            var movement = entity.GetComponent<TransformInstance>();
            if (movement == null) return;

            Vector2 randomDirection = Vector2.Normalize(
                new Vector2(Random.Shared.NextSingle() - 0.5f, Random.Shared.NextSingle() - 0.5f));

            movement.SetMovementIntent(randomDirection);
        }

        private void TickChase(
            EntityInstance entity,
            AIInstance ai)
        {
            if (string.IsNullOrEmpty(ai.TargetEntityId))
            {
                ai.AIState = AIState.ReturnHome;
                return;
            }

            var movement = entity.GetComponent<TransformInstance>();
            if (movement == null || string.IsNullOrEmpty(ai.TargetEntityId))
                return;

            var target = worldContext.GetEntity(ai.TargetEntityId);
            var targetTransform = target?.GetComponent<TransformInstance>();
            if (target == null || targetTransform == null)
            {
                ai.AIState = AIState.ReturnHome;
                return;
            }

            float distance = Vector2.Distance(movement.Position, targetTransform.Position);

            if (distance > ai.LeashDistance)
            {
                ai.TargetEntityId = null;
                ai.AIState = AIState.ReturnHome;
                return;
            }

            // Use the config range instead of hardcoded 1.5f
            float attackRange = ai.AttackRange;
            if (distance <= attackRange)
            {
                ai.AIState = AIState.Attack;
                movement?.SetMovementIntent(Vector2.Zero); // Stop moving to attack
                return;
            }

            movement?.SetMovementIntent(Vector2.Normalize(targetTransform.Position - movement.Position));
        }

        private void TickAttack(
            float dt,
            EntityInstance entity,
            AIInstance ai)
        {
            ai.AttackTimer -= dt;
            if (ai.AttackTimer > 0) return;
            if (string.IsNullOrEmpty(ai.TargetEntityId))
            {
                ai.AIState = AIState.ReturnHome;
                return;
            }

            var target = worldContext.GetEntity(ai.TargetEntityId);
            var transform = entity.GetComponent<TransformInstance>();
            if (target == null || transform == null)
            {
                ai.AIState = AIState.ReturnHome;
                return;
            }

            var targetTransform = target.GetComponent<TransformInstance>();
            if (targetTransform == null)
            {
                ai.AIState = AIState.ReturnHome;
                return;
            }

            // 1. Stop moving when preparing/executing the attack
            transform.SetMovementIntent(Vector2.Zero);

            // 2. Trigger the item action if the AI has an item equipped
            if (!string.IsNullOrEmpty(ai.EquippedItemDefinitionID))
            {
                var actionState = entity.GetComponent<ActionInstance>();
                var inventory = entity.GetComponent<InventoryInstance>();

                if (actionState != null && inventory != null)
                {
                    var itemInstance = inventory.Items.FirstOrDefault(i => i.DefinitionID == ai.EquippedItemDefinitionID);

                    if (itemInstance != null)
                    {
                        // Register the intent on ActionInstance so ItemService processes it next frame
                        actionState.SetItemUseIntent(
                            itemInstance.ID,
                            targetTransform.Position,
                            unequippedSlot: null,
                            ItemUsageAction.Use
                        );
                    }
                }
            }

            // 3. Reset attack cooldown based on stats
            var characteristic = entity.GetComponent<CharacteristicInstance>();
            float attackSpeed = characteristic?.GetCore(AttributeType.CooldownReduction) ?? 1f;
            float interval = attackSpeed <= 0f ? 1f : 1f / attackSpeed;

            ai.AttackTimer = interval;

            // If target moved out of range, go back to chasing
            if (Vector2.Distance(transform.Position, targetTransform.Position) > ai.AttackRange)
            {
                ai.AIState = AIState.Chase;
            }
        }
        #endregion
    }
}