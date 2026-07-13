using Application.Interfaces.Cache;
using Contract;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Runtime.MetaDomain;
using ResponseCode;

namespace Application.Services.UsageService
{
    public class InventoryService
    {
        #region Attributes
        private readonly ICacheProvider cacheProvider;
        #endregion

        public InventoryService(
            ICacheProvider cacheProvider)
        {
            this.cacheProvider = cacheProvider;
        }

        #region Methods
        /// <summary>
        /// Transfers all items from a source entity to a destination entity.
        /// Returns a list of any item payloads that could not fit into the destination.
        /// </summary>
        public List<ItemInstance> TransferAllItems(
            EntityInstance source,
            EntityInstance destination)
        {
            var leftOvers = new List<ItemInstance>();

            var sourceInventory = source.GetComponent<InventoryInstance>();
            if (sourceInventory == null)
                throw new InternalException(
                    ApplicationCode.InventoryServiceCode.TransferSourceInventoryMissing,
                    $"Source entity {source.ID} does not possess an InventoryInstance component.");

            // Snapshot the collection to avoid modification errors during enumeration
            var itemsToTransfer = sourceInventory.Items.ToList();

            foreach (var item in itemsToTransfer)
            {
                // Remove from the source container immediately
                sourceInventory.Items.Remove(item);

                // Attempt to inject into the destination container
                var remainder = AddItem(destination, item);

                // If the receiver's bag filled up, track the leftover payload
                if (remainder != null)
                {
                    leftOvers.Add(remainder);
                }
            }

            return leftOvers;
        }

        /// <summary>
        /// Attempts to add an item instance to an entity's inventory, respecting stack limits and slot configurations.
        /// Returns the remaining item payload if the inventory fills up, or null if fully consumed.
        /// </summary>
        public ItemInstance? AddItem(
            EntityInstance entity,
            ItemInstance item)
        {
            var inventory = entity.GetComponent<InventoryInstance>();
            if (inventory == null)
                throw new InternalException(
                    ApplicationCode.InventoryServiceCode.AddTargetInventoryMissing,
                    $"Target entity {entity.ID} does not possess an InventoryInstance component.");

            var inventoryDef = cacheProvider.Inventory.Get(inventory.DefinitionID);
            if (inventoryDef == null)
                throw new InternalException(
                    ApplicationCode.InventoryServiceCode.AddInventoryDefinitionNotFound,
                    $"Inventory definition with ID: {inventory.DefinitionID} was not found in cache");

            var itemDef = cacheProvider.Item.Get(item.DefinitionID);
            if (itemDef == null)
                throw new InternalException(
                    ApplicationCode.InventoryServiceCode.AddItemDefinitionNotFound,
                    $"Item definition with ID: {item.DefinitionID} was not found in cache");

            int remaining = item.Amount;
            int maxStack = itemDef.MaxStack ?? 1; // Default non-stackable items to a max stack of 1

            // ─────────────────────────────
            // 1. STACK FIRST
            // ─────────────────────────────
            if (maxStack > 1)
            {
                // A stack is distinct by its item definition ID and its quality variant
                foreach (var slot in inventory.Items
                    .Where(x =>
                        x.DefinitionID == item.DefinitionID &&
                        x.Quality == item.Quality))
                {
                    int canTake = maxStack - slot.Amount;

                    if (canTake <= 0)
                        continue;

                    int toAdd = Math.Min(canTake, remaining);

                    slot.AddAmount(toAdd);
                    remaining -= toAdd;

                    if (remaining <= 0)
                        return null;
                }
            }

            // ─────────────────────────────
            // 2. FILL NEW SLOTS
            // ─────────────────────────────
            while (remaining > 0)
            {
                if (inventory.Items.Count >= inventoryDef.SlotCount)
                {
                    // Inventory is completely full → return remaining details as a new payload
                    return new ItemInstance(
                        id: item.ID,
                        definitionId: item.DefinitionID,
                        amount: remaining,
                        quality: item.Quality,
                        durability: item.Durability);
                }

                int toCreate = Math.Min(maxStack, remaining);

                inventory.Items.Add(new ItemInstance(
                    id: item.ID,
                    definitionId: item.DefinitionID,
                    amount: toCreate,
                    quality: item.Quality,
                    durability: item.Durability));

                remaining -= toCreate;
            }

            return null;
        }

        /// <summary>
        /// Removes an exact reference of an item stack entirely from an entity's inventory.
        /// </summary>
        public ItemInstance RemoveItem(
            EntityInstance entity,
            ItemInstance item)
        {
            var inventory = entity.GetComponent<InventoryInstance>();
            if (inventory == null)
                throw new InternalException(
                    ApplicationCode.InventoryServiceCode.RemoveTargetInventoryMissing,
                    $"Target entity {entity.ID} does not possess an InventoryInstance component.");

            bool removed = inventory.Items.Remove(item);
            if (!removed)
            {
                throw new InternalException(
                    ApplicationCode.InventoryServiceCode.RemoveItemNotFound,
                    $"Critical State Desync: Failed to remove Item Instance from Entity {entity.ID}'s inventory collection.");
            }

            return item;
        }

        /// <summary>
        /// Deducts a (Constraint.ITEM_DEDUCTED_VALUE) quantity unit from an item stack. Splits off and returns a single unit payload, 
        /// or handles full stack removal if it was the last item.
        /// </summary>
        public ItemInstance DeductItem(
            EntityInstance entity,
            ItemInstance item)
        {
            var inventory = entity.GetComponent<InventoryInstance>();
            if (inventory == null)
                throw new InternalException(
                    ApplicationCode.InventoryServiceCode.DeductTargetInventoryMissing,
                    $"Target entity {entity.ID} does not possess an InventoryInstance component.");

            if (item.Amount > Constraint.ITEM_DEDUCTED_VALUE)
            {
                item.RemoveAmount(Constraint.ITEM_DEDUCTED_VALUE);

                return new ItemInstance(
                    id: item.ID,
                    definitionId: item.DefinitionID,
                    amount: Constraint.ITEM_DEDUCTED_VALUE,
                    quality: item.Quality,
                    durability: item.Durability);
            }

            inventory.Items.Remove(item);
            return item;
        }

        /// <summary>
        /// Lowers the durability tracking values of a target item, removing it entirely if it breaks.
        /// </summary>
        public void DegradeItem(
            EntityInstance entity,
            ItemInstance item)
        {
            if (item.Durability.HasValue)
            {
                bool isShattered = item.DegradeDurability();
                if (isShattered)
                {
                    RemoveItem(entity, item);
                }
            }
        }

        /// <summary>
        /// Simulates item injection to verify if a given item payload completely fits inside the entity container space.
        /// </summary>
        public bool CanAddItem(
            EntityInstance entity,
            ItemInstance item)
        {
            var inventory = entity.GetComponent<InventoryInstance>();
            if (inventory == null)
                throw new InternalException(
                    ApplicationCode.InventoryServiceCode.CanAddTargetInventoryMissing,
                    $"Target entity {entity.ID} does not possess an InventoryInstance component.");

            var inventoryDef = cacheProvider.Inventory.Get(inventory.DefinitionID);
            if (inventoryDef == null)
                throw new InternalException(
                    ApplicationCode.InventoryServiceCode.CanAddInventoryDefinitionNotFound,
                    $"Inventory definition with ID: {inventory.DefinitionID} was not found in cache");

            var itemDef = cacheProvider.Item.Get(item.DefinitionID);
            if (itemDef == null)
                throw new InternalException(
                    ApplicationCode.InventoryServiceCode.CanAddItemDefinitionNotFound,
                    $"Item definition with ID: {item.DefinitionID} was not found in cache");

            int remaining = item.Amount;
            int maxStack = itemDef.MaxStack ?? 1;

            // ─────────────────────────────
            // 1. STACK CHECK
            // ─────────────────────────────
            if (maxStack > 1)
            {
                foreach (var slot in inventory.Items
                    .Where(x =>
                        x.DefinitionID == item.DefinitionID &&
                        x.Quality == item.Quality))
                {
                    int canTake = maxStack - slot.Amount;

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
            int freeSlots = inventoryDef.SlotCount - inventory.Items.Count;
            int neededSlots = (int)Math.Ceiling(remaining / (float)maxStack);

            return neededSlots <= freeSlots;
        }
        #endregion
    }
}