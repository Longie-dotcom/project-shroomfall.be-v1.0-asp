using Application.Interfaces.Cache;
using Application.Services.AttributeService;
using Contract.Enum.EntityDomain;
using Domain.Definition.ItemDomain;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.ItemDomain;
using Domain.Shared;

namespace Application.Services.ItemService
{
    public class EquipmentService
    {
        #region Attributes
        private readonly InventoryService inventoryService;
        private readonly EffectService effectService;
        private readonly IItemCache itemCache;
        #endregion

        #region Properties
        #endregion

        public EquipmentService(
            InventoryService inventoryService,
            EffectService effectService,
            IItemCache itemCache)
        {
            this.inventoryService = inventoryService;
            this.effectService = effectService;
            this.itemCache = itemCache;
        }

        #region Methods
        public void Equip(
            CreatureInstance creature,
            ItemInstance item,
            Item itemDef)
        {
            var inventory = creature.Inventory;

            // Validate item and resolve slot
            bool isEquipped = EquipmentMapping.Map.TryGetValue(itemDef.Category, out var slot);
            if (!isEquipped)
                throw new BadRequest(
                    ResponseCode.EquipmentService_InvalidItem,
                    $"Item is not an equipment, item defintion ID: {item.DefinitionID}, item instance ID: {item.ID}");

            // Prevent replacing equipped item
            if (creature.GetEquipment(slot) != null)
                throw new BadRequest(
                    ResponseCode.EquipmentService_EquipmentSlotOccupied,
                    $"Equipment slot has been occupied");

            // Remove from inventory
            var grabbed = inventoryService.RemoveForEquip(creature, item.ID);

            // Equip item
            creature.SetEquipment(slot, grabbed);

            // Apply effects
            effectService.ApplyItemEffects(creature, itemDef, grabbed.ID);
        }

        public void Unequip(
            CreatureInstance creature,
            EquipmentSlot slot)
        {
            var equipped = creature.GetEquipment(slot);
            if (equipped == null)
                return;

            // Validate existed space to return back to inventory
            if (!inventoryService.CanAddItem(creature, equipped))
                throw new BadRequest(
                    ResponseCode.EquipmentService_InventoryFullOnUnequip,
                    $"Inventory is full, can not unequipped equipment");

            // Find item definition from cache
            var itemDef = itemCache.Get(equipped.DefinitionID);
            if (itemDef == null)
                throw new InternalException(
                    ResponseCode.EquipmentService_ItemDefinitionNotFound,
                    $"Item defintion ID: {equipped.DefinitionID} was not found");

            // Remove equipment
            creature.RemoveEquipment(slot);

            // Remove effects
            effectService.RemoveItemEffects(creature, equipped.ID);

            // Add back to inventory
            var remainder = inventoryService.AddItem(creature, equipped);
            if (remainder != null)
            {
                // Rollback safety
                creature.SetEquipment(slot, equipped);
                effectService.ApplyItemEffects(creature, itemDef, equipped.ID);

                throw new InternalException(
                    ResponseCode.EquipmentService_InventoryFullOnUnequip,
                    $"Inventory is full, can not unequipped equipment");
            }
        }

        public void RehydrateEquipment(
            CreatureInstance creature,
            Dictionary<EquipmentSlot, ItemInstance?> equipment)
        {
            foreach (var kv in equipment)
            {
                if (kv.Value == null)
                    continue;

                var item = new ItemInstance(
                    kv.Value.ID,
                    kv.Value.DefinitionID,
                    kv.Value.Count,
                    kv.Value.CurrentDurability,
                    kv.Value.Quality);

                creature.SetEquipment(kv.Key, item);
            }
        }
        #endregion
    }
}