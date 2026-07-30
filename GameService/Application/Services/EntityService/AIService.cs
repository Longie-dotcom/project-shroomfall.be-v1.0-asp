using Application.Services.WorldService;
using Application.Systems.Abstraction;
using Application.Systems.Queue;
using Contract.Enum.EntityDomain;
using Contract.Enum.MetaDomain.Effect;
using Contract.Enum.MetaDomain.Item;
using Domain.Common;
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

                ai.ThinkCooldownRemaining = ai.ThinkInterval;

                Console.WriteLine($"[AIService] Entity '{entity.ID}' processing state: {ai.AIState}");

                switch (ai.AIState)
                {
                    case AIState.Idle: TickIdle(entity, ai); break;
                    case AIState.Wander: TickWander(entity, ai); break;
                    case AIState.Chase: TickChase(entity, ai); break;
                    case AIState.Attack: TickAttack(dt, entity, ai); break;
                    default:
                        Console.WriteLine($"[AIService] Entity '{entity.ID}' is in unhandled state: {ai.AIState}");
                        break;
                }
            }
        }

        // NEW: Extracted target finding logic so both Idle and Wander can use it
        private bool TryFindPlayer(EntityInstance entity, AIInstance ai, TransformInstance transform)
        {
            var detectionShape = new CircleShape(ai.AggroRadius, isBlocking: false);

            var body = new CollisionBody(
                entity.ID,
                transform.RoomSpatialID,
                transform.Position,
                new Vector2(0, 0),
                transform.LayerZ,
                detectionShape,
                CollisionLayer.None,
                CollisionLayer.Player);

            var collision = collisionService.QueryOverlap(body, transform.Position);

            Console.WriteLine($"[AIService - Vision] Entity '{entity.ID}' queried player collision at {transform.Position} (Radius: {ai.AggroRadius}). Detected {collision.Entities.Count} entities.");

            foreach (var hit in collision.Entities)
            {
                ai.TargetEntityId = hit.ID;
                return true;
            }

            return false;
        }

        private void TickIdle(
            EntityInstance entity,
            AIInstance ai)
        {
            var transform = entity.GetComponent<TransformInstance>();
            if (transform == null)
            {
                Console.WriteLine($"[AIService - Idle] Entity '{entity.ID}' missing TransformInstance.");
                return;
            }

            // Check for player
            if (TryFindPlayer(entity, ai, transform))
            {
                ai.AIState = AIState.Chase;
                Console.WriteLine($"[AIService - Idle] Entity '{entity.ID}' detected Player target '{ai.TargetEntityId}'! Changing state to Chase.");
                return;
            }

            Console.WriteLine($"[AIService - Idle] Entity '{entity.ID}' found no player target. Changing state to Wander.");
            ai.AIState = AIState.Wander;
        }

        private void TickWander(
            EntityInstance entity,
            AIInstance ai) // Added AIInstance parameter
        {
            var movement = entity.GetComponent<TransformInstance>();
            if (movement == null)
            {
                Console.WriteLine($"[AIService - Wander] Entity '{entity.ID}' missing TransformInstance.");
                return;
            }

            // 1. Check for the player while wandering!
            if (TryFindPlayer(entity, ai, movement))
            {
                ai.AIState = AIState.Chase;
                Console.WriteLine($"[AIService - Wander] Entity '{entity.ID}' detected Player while wandering! Changing state to Chase.");
                return;
            }

            // 2. If no player, wander
            Vector2 randomDirection = Vector2.Normalize(
                new Vector2(Random.Shared.NextSingle() - 0.5f, Random.Shared.NextSingle() - 0.5f));

            movement.SetMovementIntent(randomDirection);
            Console.WriteLine($"[AIService - Wander] Entity '{entity.ID}' wandering towards Direction: {randomDirection.X:F2}: {randomDirection.Y:F2}");

            // 3. Return to idle so it stops and looks around next tick, rather than getting stuck in Wander
            ai.AIState = AIState.Idle;
        }

        private void TickChase(
            EntityInstance entity,
            AIInstance ai)
        {
            var movement = entity.GetComponent<TransformInstance>();
            if (movement == null || string.IsNullOrEmpty(ai.TargetEntityId))
            {
                Console.WriteLine($"[AIService - Chase] Entity '{entity.ID}' missing TransformInstance or TargetEntityId is null/empty.");
                return;
            }

            var target = worldContext.GetEntity(ai.TargetEntityId);
            var targetTransform = target?.GetComponent<TransformInstance>();
            if (target == null || targetTransform == null)
            {
                Console.WriteLine($"[AIService - Chase] Entity '{entity.ID}' target '{ai.TargetEntityId}' not found in world context or missing TransformInstance. Returning Home.");
                movement.ClearMovementIntent();
                ai.TargetEntityId = null;
                ai.AIState = AIState.Idle; // Changed to Idle instead of ReturnHome unless you have a specific ReturnHome state logic
                return;
            }

            float distance = Vector2.Distance(movement.Position, targetTransform.Position);
            Console.WriteLine($"[AIService - Chase] Entity '{entity.ID}' chasing target '{target.ID}'. Current Distance: {distance:F2} | Leash: {ai.LeashDistance} | AttackRange: {ai.AttackRange}");

            if (distance > ai.LeashDistance)
            {
                Console.WriteLine($"[AIService - Chase] Entity '{entity.ID}' target exceeded LeashDistance. Losing aggro and going Idle.");
                ai.TargetEntityId = null;
                ai.AIState = AIState.Idle; // Changed to Idle
                movement.ClearMovementIntent();
                return;
            }

            float attackRange = ai.AttackRange;
            if (distance <= attackRange)
            {
                Console.WriteLine($"[AIService - Chase] Entity '{entity.ID}' reached AttackRange ({distance:F2} <= {attackRange}). Changing state to Attack.");
                ai.AIState = AIState.Attack;
                movement.ClearMovementIntent();
                return;
            }

            Vector2 moveDir = Vector2.Normalize(targetTransform.Position - movement.Position);
            movement.SetMovementIntent(moveDir);
            Console.WriteLine($"[AIService - Chase] Entity '{entity.ID}' moving towards target. Pos: {movement.Position}, TargetPos: {targetTransform.Position}, Intent: {moveDir}");
        }

        private void TickAttack(
            float dt,
            EntityInstance entity,
            AIInstance ai)
        {
            var transform = entity.GetComponent<TransformInstance>();
            if (transform == null || string.IsNullOrEmpty(ai.TargetEntityId))
            {
                Console.WriteLine($"[AIService - Attack] Entity '{entity.ID}' missing TransformInstance or TargetEntityId is null.");
                ai.AIState = AIState.Idle;
                return;
            }

            var target = worldContext.GetEntity(ai.TargetEntityId);
            if (target == null)
            {
                Console.WriteLine($"[AIService - Attack] Entity '{entity.ID}' target null. Going Idle.");
                transform.ClearMovementIntent();
                ai.AIState = AIState.Idle;
                return;
            }

            var targetTransform = target.GetComponent<TransformInstance>();
            if (targetTransform == null)
            {
                Console.WriteLine($"[AIService - Attack] Entity '{entity.ID}' target missing TransformInstance. Going Idle.");
                transform.ClearMovementIntent();
                ai.AIState = AIState.Idle;
                return;
            }

            // 1. Stop moving when preparing/executing the attack
            transform.ClearMovementIntent();

            ai.AttackTimer -= dt;
            if (ai.AttackTimer > 0)
            {
                Console.WriteLine($"[AIService - Attack] Entity '{entity.ID}' attack on cooldown ({ai.AttackTimer:F2}s left).");
                return;
            }

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
                        Console.WriteLine($"[AIService - Attack] Entity '{entity.ID}' triggering attack intent with item '{itemInstance.ID}'.");
                        actionState.SetItemUseIntent(
                            itemInstance.ID,
                            targetTransform.Position,
                            unequippedSlot: null,
                            ItemUsageAction.Use
                        );
                    }
                    else
                    {
                        Console.WriteLine($"[AIService - Attack] Entity '{entity.ID}' equipped item definition ID '{ai.EquippedItemDefinitionID}' not found in inventory.");
                    }
                }
                else
                {
                    Console.WriteLine($"[AIService - Attack] Entity '{entity.ID}' missing ActionInstance or InventoryInstance.");
                }
            }
            else
            {
                Console.WriteLine($"[AIService - Attack] Entity '{entity.ID}' executing attack without equipped item definition.");
            }

            // 3. Reset attack cooldown based on stats
            var characteristic = entity.GetComponent<CharacteristicInstance>();
            float attackSpeed = characteristic?.GetCore(AttributeType.CooldownReduction) ?? 1f;
            float interval = attackSpeed <= 0f ? 1f : 1f / attackSpeed;

            ai.AttackTimer = interval;

            // If target moved out of range, go back to chasing
            float currentDist = Vector2.Distance(transform.Position, targetTransform.Position);
            if (currentDist > ai.AttackRange)
            {
                Console.WriteLine($"[AIService - Attack] Target moved out of range ({currentDist:F2} > {ai.AttackRange}). Switching back to Chase.");
                ai.AIState = AIState.Chase;
            }
        }
        #endregion
    }
}