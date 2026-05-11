using Application.Services.Abstraction.AttributeService;
using Application.Services.Abstraction.ItemService;
using Domain.Definition.ItemDomain;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.ItemDomain;

namespace Application.Services.ItemService
{
    public class ConsumableService : IConsumableService
    {
        #region Attributes
        private readonly IEffectService effectService;
        private readonly IInventoryService inventoryService;
        #endregion

        #region Properties
        #endregion

        public ConsumableService(
            IEffectService effectService,
            IInventoryService inventoryService)
        {
            this.effectService = effectService;
            this.inventoryService = inventoryService;
        }

        #region Methods
        public void Consume(
            CreatureInstance creature, 
            ItemInstance item, 
            Item itemDef)
        {
            // Remove from inventory
            var grabbed = inventoryService.RemoveForConsume(creature, item.ID);

            // Apply effects
            effectService.ApplyItemEffects(creature, itemDef, grabbed.ID);
        }
        #endregion
    }
}