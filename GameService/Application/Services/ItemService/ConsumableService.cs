using Application.Services.AttributeService;
using Domain.Definition.ItemDomain;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.ItemDomain;

namespace Application.Services.ItemService
{
    public class ConsumableService
    {
        #region Attributes
        private readonly EffectService effectService;
        private readonly InventoryService inventoryService;
        #endregion

        #region Properties
        #endregion

        public ConsumableService(
            EffectService effectService,
            InventoryService inventoryService)
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