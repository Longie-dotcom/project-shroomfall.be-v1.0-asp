using Application.Services.AttributeService;
using Application.Services.WorldService;
using Application.Systems.Queue;

namespace Application.Systems.System
{
    public class EntityResolver
    {
        #region Attributes
        private readonly WorldContext worldContext;
        private readonly CollisionService collisionService;
        private readonly DeathService deathService;
        #endregion

        #region Properties
        #endregion

        public EntityResolver(
            WorldContext worldContext,
            CollisionService collisionService,
            DeathService deathService)
        {
            this.worldContext = worldContext;
            this.collisionService = collisionService;
            this.deathService = deathService;
        }

        #region Methods
        public void Resolve(
            CommandBuffer commandBuffer)
        {
            while (commandBuffer.Commands.TryDequeue(out var command))
            {
                switch (command)
                {
                    case MovementCommand moveCmd:
                        ResolveMovement(moveCmd, commandBuffer);
                        break;

                    case ItemActionCommand itemCmd:
                        ResolveItemAction(itemCmd, commandBuffer);
                        break;

                    case EntityExpiredCommand entityExpiredCmd:
                        ResolveEntityExpired(entityExpiredCmd, commandBuffer);
                        break;

                    case VitalThresholdCommand vitalThresholdCmd:
                        ResolveVitalThreshold(vitalThresholdCmd, commandBuffer);
                        break;

                    case EntityDespawnCommand entityDespawnCmd:
                        ResolveEntityDespawn(entityDespawnCmd, commandBuffer);
                        break;
                }
            }
        }

        private void ResolveMovement(
            MovementCommand cmd,
            CommandBuffer commandBuffer)
        {
            var roomSpatial = worldContext.GetRoom(cmd.Body.RoomSpatialID);
            if (roomSpatial == null) 
                return;

            // Query the spatial hash/grid for collisions
            var collision = collisionService.QueryMovement(cmd.Body, cmd.DesiredPosition);

            // Calculate the actual final position based on blocks
            var finalPos = cmd.DesiredPosition;
            if (collision.BlockX) finalPos.X = cmd.Body.Position.X;
            if (collision.BlockY) finalPos.Y = cmd.Body.Position.Y;

            Console.WriteLine(
    $"Desired={cmd.DesiredPosition.X}, {cmd.DesiredPosition.Y}  " +
    $"BlockX={collision.BlockX} BlockY={collision.BlockY}");

            // Enqueue the validated movement
            commandBuffer.Results.Enqueue(new MovementResult(
                cmd.EntityInstanceID,
                finalPos,
                collision.LayerZ,
                collision.Triggers));
        }

        private void ResolveItemAction(
            ItemActionCommand cmd, 
            CommandBuffer commandBuffer)
        {
            commandBuffer.Results.Enqueue(new ItemActionResult(
                cmd.EntityInstanceID,
                cmd.Context));
        }

        public void ResolveEntityExpired(
            EntityExpiredCommand cmd,
            CommandBuffer commandBuffer)
        {
            commandBuffer.Results.Enqueue(new EntityExpiredResult(
                cmd.EntityInstanceID));
        }

        private void ResolveVitalThreshold(
            VitalThresholdCommand cmd,
            CommandBuffer commandBuffer)
        {
            var entity = worldContext.GetEntity(cmd.EntityInstanceID);
            if (entity == null)
                return;

            var deathOutcome = deathService.CheckDeath(
                entity,
                cmd.Vital,
                cmd.PreviousValue,
                cmd.CurrentValue);

            commandBuffer.Results.Enqueue(new VitalThresholdResult(
                cmd.EntityInstanceID,
                deathOutcome));
        }

        private void ResolveEntityDespawn(
            EntityDespawnCommand cmd,
            CommandBuffer commandBuffer)
        {
            commandBuffer.Results.Enqueue(new EntityDespawnResult(
                cmd.EntityInstanceID,
                cmd.TriggerDeathLogic));
        }
        #endregion
    }
}