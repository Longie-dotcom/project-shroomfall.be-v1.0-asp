using Application.Interfaces.Cache;
using Application.Interfaces.Factory;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.ItemDomain;
using Domain.Shared;

namespace Application.Services.ItemService
{
    public class InventoryService
    {
        #region Attributes
        private readonly IInventoryCache inventoryCache;
        private readonly IItemCache itemCache;
        private readonly IItemInstanceFactory itemFactory;
        #endregion

        #region Properties
        #endregion

        public InventoryService(
            IInventoryCache inventoryCache,
            IItemCache itemCache,
            IItemInstanceFactory itemFactory)
        {
            this.inventoryCache = inventoryCache;
            this.itemCache = itemCache;
            this.itemFactory = itemFactory;
        }

        #region Methods
        public ItemInstance? AddItem(
            CreatureInstance creature, 
            ItemInstance item)
        {
            var inventory = creature.Inventory;

            var inventoryDef = inventoryCache.Get(inventory.DefinitionID);
            if (inventoryDef == null)
                throw new InternalException(
                    ResponseCode.InventoryService_DefinitionNotFound,
                    $"Inventory definition with ID: {inventory.DefinitionID} was not found in cache"
);

            var itemDef = itemCache.Get(item.DefinitionID);
            if (itemDef == null)
                throw new InternalException(
                    ResponseCode.InventoryService_ItemDefinitionNotFound,
                    $"Item definition with ID: {item.DefinitionID} was not found in cache");

            int remaining = item.Count;

            // ─────────────────────────────
            // 1. STACK FIRST
            // ─────────────────────────────
            if (itemDef.Stackable)
            {
                // A stack is distinct by definition and quality
                foreach (var slot in inventory.Items
                    .Where(x =>
                        x.DefinitionID == item.DefinitionID &&
                        x.Quality == item.Quality))
                {
                    int canTake = Constraint.MAX_ITEM_AMOUNT_PER_SLOT - slot.Count;

                    if (canTake <= 0)
                        continue;

                    int toAdd = Math.Min(canTake, remaining);

                    slot.Add(toAdd);
                    remaining -= toAdd;

                    if (remaining <= 0)
                        return null;
                }
            }

            // ─────────────────────────────
            // 2. NEW SLOTS
            // ─────────────────────────────
            while (remaining > 0)
            {
                if (inventory.Items.Count >= inventoryDef.SlotCount)
                {
                    // Inventory full → return remainder
                    return itemFactory.Create(
                        definitionId: item.DefinitionID,
                        count: remaining,
                        currentDurability: item.CurrentDurability,
                        quality: item.Quality
                    );
                }

                int toCreate = Math.Min(Constraint.MAX_ITEM_AMOUNT_PER_SLOT, remaining);

                inventory.Items.Add(itemFactory.Create(
                    item.DefinitionID,
                    toCreate,
                    item.CurrentDurability,
                    item.Quality
                ));

                remaining -= toCreate;
            }

            return null;
        }

        public ItemInstance RemoveForEquip(
            CreatureInstance creature,
            string itemInstanceId)
        {
            var inventory = creature.Inventory;

            var item = GetItemOrThrow(creature, itemInstanceId);

            // Equipment must always be single-instance items
            if (item.Count != 1)
                throw new BadRequest(
                    ResponseCode.InventoryService_InvalidEquipItem,
                    $"Item with instance ID: {item.ID} cannot be equipped because stack count is greater than 1");

            inventory.Items.Remove(item);

            return item;
        }

        public ItemInstance RemoveForConsume(
            CreatureInstance creature,
            string itemInstanceId)
        {
            var inventory = creature.Inventory;

            var item = GetItemOrThrow(creature, itemInstanceId);

            if (item.Count > 1)
            {
                item.Remove(1);

                return itemFactory.Create(
                    item.DefinitionID,
                    1,
                    item.CurrentDurability,
                    item.Quality
                );
            }

            inventory.Items.Remove(item);

            return item;
        }

        public bool CanAddItem(
            CreatureInstance creature,
            ItemInstance item)
        {
            var inventory = creature.Inventory;

            var inventoryDef = inventoryCache.Get(inventory.DefinitionID);
            if (inventoryDef == null)
                throw new InternalException(
                    ResponseCode.InventoryService_DefinitionNotFound,
                    $"Inventory definition with ID: {inventory.DefinitionID} was not found in cache");

            var itemDef = itemCache.Get(item.DefinitionID);
            if (itemDef == null)
                throw new InternalException(
                    ResponseCode.InventoryService_ItemDefinitionNotFound,
                    $"Item definition with ID: {item.DefinitionID} was not found in cache");

            int remaining = item.Count;

            // ─────────────────────────────
            // 1. STACK CHECK
            // ─────────────────────────────
            if (itemDef.Stackable)
            {
                // A stack is distinct by definition and quality
                foreach (var slot in inventory.Items
                    .Where(x =>
                        x.DefinitionID == item.DefinitionID &&
                        x.Quality == item.Quality))
                {
                    int canTake = Constraint.MAX_ITEM_AMOUNT_PER_SLOT - slot.Count;

                    if (canTake <= 0)
                        continue;

                    remaining -= Math.Min(canTake, remaining);

                    if (remaining <= 0)
                        return true;
                }
            }

            // ─────────────────────────────
            // 2. SLOT CHECK
            // ─────────────────────────────
            int freeSlots =
                inventoryDef.SlotCount - inventory.Items.Count;

            int neededSlots =
                (int)Math.Ceiling(
                    remaining / (float)Constraint.MAX_ITEM_AMOUNT_PER_SLOT);

            return neededSlots <= freeSlots;
        }

        private ItemInstance GetItemOrThrow(
            CreatureInstance creature, 
            string itemInstanceId)
        {
            var inventory = creature.Inventory;

            var item = inventory.Items.FirstOrDefault(x => x.ID == itemInstanceId);
            if (item == null)
                throw new BadRequest(
                    ResponseCode.InventoryService_ItemNotFound,
                    $"Item with instance ID: {itemInstanceId} was not found in creature: {creature.ID}" +
                    $" (Def ID: {creature.DefinitionID}) inventory");

            return item;
        }
        #endregion
    }
}