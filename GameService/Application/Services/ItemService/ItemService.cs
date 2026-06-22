using Application.Context;
using Application.Systems.Abstraction;
using Application.Systems.Queue;
using Domain.Runtime.EntityDomain.Component;

namespace Application.Services.ItemService
{
    public class ItemService : ITickService
    {
        #region Attributes
        private readonly WorldContext worldContext;
        #endregion

        #region Properties
        #endregion

        public ItemService(
            WorldContext worldContext)
        {
            this.worldContext = worldContext;
        }

        #region Methods
        public void Tick(
            float dt, 
            CommandBuffer commandBuffer)
        {
            var entities = worldContext.GetEntities().ToList();

            foreach (var entity in entities)
            {
                var actionState = entity.GetComponent<ActionInstance>();

                if (actionState != null && actionState.PendingItemUseID != null)
                {
                    // Enqueue the command for the Resolver
                    commandBuffer.Commands.Enqueue(new ItemActionCommand(
                        entity.ID,
                        actionState.PendingItemUseID,
                        actionState.PendingTargetPosition));

                    // Clear the intent
                    actionState.ClearItemUseIntent();
                }
            }
        }
        #endregion
    }
}