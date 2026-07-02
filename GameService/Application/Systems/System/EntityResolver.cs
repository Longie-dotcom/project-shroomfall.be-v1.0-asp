using Application.Interfaces.Cache;
using Application.Services.WorldService;
using Application.Systems.Queue;
using Domain.Runtime.EntityDomain.Component;

namespace Application.Systems.System
{
    public class EntityResolver
    {
        #region Attributes
        private readonly WorldContext worldContext;
        private readonly ICacheProvider cacheProvider;
        private readonly CollisionService collisionService;
        private readonly EntitySpawnService entitySpawnService;
        #endregion

        #region Properties
        #endregion

        public EntityResolver(
            WorldContext worldContext,
            ICacheProvider cacheProvider,
            CollisionService collisionService,
            EntitySpawnService entitySpawnService)
        {
            this.worldContext = worldContext;
            this.cacheProvider = cacheProvider;
            this.collisionService = collisionService;
            this.entitySpawnService = entitySpawnService;
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
                }
            }
        }

        private void ResolveMovement(
            MovementCommand cmd,
            CommandBuffer commandBuffer)
        {
            var roomSpatial = worldContext.GetRoom(cmd.Body.RoomSpatialID);
            if (roomSpatial == null) return;

            // Query the spatial hash/grid for collisions
            var collision = collisionService.QueryMovement(cmd.Body, cmd.DesiredPosition);

            // Calculate the actual final position based on blocks
            var finalPos = cmd.DesiredPosition;
            if (collision.BlockX) finalPos.X = cmd.Body.Position.X;
            if (collision.BlockY) finalPos.Y = cmd.Body.Position.Y;

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
            var entity = worldContext.GetEntity(cmd.EntityInstanceID);
            if (entity == null) return;

            // Validate the inventory to prevent race conditions or duplicate usage
            var inventory = entity.GetComponent<InventoryInstance>();
            if (inventory == null) return;

            var item = inventory.Items.FirstOrDefault(x => x.ID == cmd.ItemInstanceID);
            if (item == null) return; // Item was already consumed or dropped!

            // Validate definition exists
            var itemDef = cacheProvider.Item.Get(item.DefinitionID);
            if (itemDef == null) return;

            // Push to the trigger phase to actually consume and spawn
            commandBuffer.Results.Enqueue(new ItemActionResult(
                cmd.EntityInstanceID,
                cmd.ItemInstanceID,
                cmd.TargetPosition));
        }

        public void ResolveEntityExpired(
            EntityExpiredCommand cmd,
            CommandBuffer commandBuffer)
        {
            var entity = worldContext.GetEntity(cmd.EntityInstanceID);
            if (entity == null) return;

            // Perform the Despawn
            entitySpawnService.Despawn(entity);

            commandBuffer.Results.Enqueue(new DespawnResult(cmd.EntityInstanceID));
        }
        #endregion
    }
}