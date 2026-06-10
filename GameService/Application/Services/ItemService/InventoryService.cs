using Application.Interfaces.Cache;
using Application.Interfaces.Factory;
using Contract;
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
        public List<ItemInstance> TransferAllItems(
            CreatureInstance source,
            CreatureInstance destination)
        {
            var leftOvers = new List<ItemInstance>();

            // Snapshot the collection to avoid modification errors during enumeration
            var itemsToTransfer = source.Inventory.Items.ToList();

            foreach (var item in itemsToTransfer)
            {
                // Remove from the dying creature/container right away
                source.Inventory.Items.Remove(item);

                // Attempt to inject into the killer/looter
                var remainder = AddItem(destination, item);

                // If the receiver's bag filled up, track the leftover to drop on the floor
                if (remainder != null)
                {
                    leftOvers.Add(remainder);
                }
            }

            return leftOvers;
        }

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

        public ItemInstance RemoveItem(
            CreatureInstance creature,
            ItemInstance item)
        {
            var inventory = creature.Inventory;

            bool removed = inventory.Items.Remove(item);
            if (!removed)
            {
                throw new InternalException(
                    ResponseCode.InventoryService_ItemNotFound,
                    $"Critical State Desync: Failed to remove Item Instance {item.ID} from Creature {creature.ID}'s inventory collection.");
            }

            return item;
        }

        public ItemInstance DeductItem(
            CreatureInstance creature,
            ItemInstance item)
        {
            var inventory = creature.Inventory;

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

        public void DegradeItem(
            CreatureInstance creature,
            ItemInstance item)
        {
            if (item.CurrentDurability.HasValue)
            {
                bool isShattered = item.DegradeDurability(1);
                if (isShattered)
                {
                    RemoveItem(creature, item);
                }
            }
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
        #endregion
    }
}