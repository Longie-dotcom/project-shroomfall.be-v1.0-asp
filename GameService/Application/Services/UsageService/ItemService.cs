using Application.Interfaces.Cache;
using Application.Services.AttributeService;
using Application.Services.WorldService;
using Application.Systems.Abstraction;
using Application.Systems.Queue;
using Contract.Enum.MetaDomain.Item;
using Domain.Common;
using Domain.Definition.MetaDomain;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Runtime.MetaDomain;
using ResponseCode;

namespace Application.Services.UsageService
{
    public class ItemUsageActionContext
    {
        public ItemInstance Item { get; }
        public ItemDefinition ItemDef { get; }
        public ItemUsageAction Type { get; }
        public Vector2? TargetVector { get; }
        public EquipmentSlot? Slot { get; }

        public ItemUsageActionContext(
            ItemInstance item,
            ItemDefinition itemDef,
            ItemUsageAction type,
            Vector2? targetVector = null,
            EquipmentSlot? slot = null)
        {
            Item = item;
            ItemDef = itemDef;
            Type = type;
            TargetVector = targetVector;
            Slot = slot;
        }
    }

    public class ItemService : ITickService
    {
        #region Attributes
        private readonly WorldContext worldContext;
        private readonly ICacheProvider cacheProvider;
        private readonly EntitySpawnService entitySpawnService;
        private readonly EffectService effectService;
        private readonly InventoryService inventoryService;
        #endregion

        #region Properties
        #endregion

        public ItemService(
            WorldContext worldContext,
            ICacheProvider cacheProvider,
            EntitySpawnService entitySpawnService,
            EffectService effectService,
            InventoryService inventoryService)
        {
            this.worldContext = worldContext;
            this.cacheProvider = cacheProvider;
            this.entitySpawnService = entitySpawnService;
            this.effectService = effectService;
            this.inventoryService = inventoryService;
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
                    var inventory = entity.GetComponent<InventoryInstance>();
                    var item = inventory?.Items.FirstOrDefault(i => i.ID == actionState.PendingItemUseID);

                    if (item != null)
                    {
                        var itemDef = cacheProvider.Item.Get(item.DefinitionID);
                        if (itemDef != null)
                        {
                            // Create the correct usage action object based on the enum intent and available data
                            ItemUsageActionContext context = new ItemUsageActionContext(
                                item, 
                                itemDef,
                                actionState.ItemUsageAction, 
                                actionState.PendingTargetPosition,
                                actionState.PendingUnequippedSlot);

                            // Enqueue the command with the typed action object
                            commandBuffer.Commands.Enqueue(new ItemActionCommand(
                                entity.ID,
                                context));
                        }
                    }

                    // Clear the intent
                    actionState.ClearItemUseIntent();
                }
            }
        }
        #endregion

        #region Core Pipeline
        /// <summary>
        /// Orchestrates the decoupled Manifestation and Cost logic.
        /// </summary>
        public void Execute(
            EntityInstance entity,
            ItemUsageActionContext action)
        {
            // Material is for crafting and trading only!
            if (action.ItemDef.Category == ItemCategory.Material)
                return;

            switch (action.Type)
            {
                case ItemUsageAction.Unequip:
                    ExecuteUnequip(
                        entity,
                        action.Slot!.Value);
                    break;

                case ItemUsageAction.Use:
                    ExecuteManifestation(
                        entity,
                        action.Item,
                        action.ItemDef,
                        action.TargetVector!);

                    DeductCost(
                        entity,
                        action.Item,
                        action.ItemDef);

                    break;
            }
        }
        #endregion

        #region Private Manifestation Steps (World Changes)
        private void ExecuteManifestation(
            EntityInstance entity,
            ItemInstance item,
            ItemDefinition itemDef,
            Vector2 targetVector)
        {
            switch (itemDef.Category)
            {
                case ItemCategory.Equippable:
                    if (itemDef.EquippableConfig != null)
                    {
                        ExecuteEquippable(entity, itemDef.EquippableConfig, item);
                    }
                    break;

                case ItemCategory.Placeable:
                    if (itemDef.PlaceableConfig != null)
                    {
                        ExecutePlaceable(entity, itemDef.PlaceableConfig, targetVector);
                    }
                    break;

                case ItemCategory.Ranged:
                    if (itemDef.RangedConfig != null)
                    {
                        ExecuteRanged(entity, itemDef.RangedConfig, targetVector);
                    }
                    break;

                case ItemCategory.Melee:
                    if (itemDef.MeleeConfig != null)
                    {
                        ExecuteMelee(entity, itemDef.MeleeConfig, targetVector);
                    }
                    break;

                case ItemCategory.Consumable:
                    if (itemDef.ConsumableConfig != null)
                    {
                        ExecuteConsumable(entity, itemDef.ConsumableConfig);
                    }
                    break;

                case ItemCategory.Material:
                    break;
            }
        }

        private void ExecuteEquippable(
            EntityInstance entity,
            EquippableConfig config,
            ItemInstance item)
        {
            var inventory = entity.GetComponent<InventoryInstance>();
            if (inventory == null)
                return;

            var slot = config.Slot;
            if (inventory.GetEquipped(slot) != null)
                ExecuteUnequip(entity, slot);

            foreach (var effectId in config.EffectDefinitionIDs)
            {
                effectService.ApplyEffect(entity, effectId);
            }

            // Equip the new item
            inventory.Equip(item, slot);
        }

        private void ExecuteUnequip(
            EntityInstance entity,
            EquipmentSlot slot)
        {
            var inventory = entity.GetComponent<InventoryInstance>();
            if (inventory == null)
                return;

            var equippedItem = inventory.GetEquipped(slot);
            if (equippedItem == null)
                return;

            // Directly remove the equipment's effects if they exist
            var itemDef = cacheProvider.Item.Get(equippedItem.DefinitionID);
            if (itemDef?.EquippableConfig?.EffectDefinitionIDs != null)
            {
                foreach (var effectId in itemDef.EquippableConfig.EffectDefinitionIDs)
                {
                    effectService.RemoveEffect(entity, effectId);
                }
            }

            inventory.Unequip(slot);
        }

        private void ExecutePlaceable(
            EntityInstance entity,
            PlaceableConfig config,
            Vector2 targetVector)
        {
            var transform = entity.GetComponent<TransformInstance>();
            if (transform == null)
                throw new InternalException(
                    ApplicationCode.ItemServiceCode.PlaceableMissingTransform,
                    $"Entity {entity.ID} missing TransformInstance when placing object.");

            var spawnContext = new WorldEntityCreateContext(
                Guid.NewGuid().ToString(),
                config.EntityDefinitionID,
                transform.RoomSpatialID,
                transform.LayerZ,
                targetVector
            );

            entitySpawnService.Spawn(spawnContext);
        }

        private void ExecuteRanged(
            EntityInstance entity,
            RangedConfig config,
            Vector2 targetVector)
        {
            var transform = entity.GetComponent<TransformInstance>();
            if (transform == null)
                throw new InternalException(
                    ApplicationCode.ItemServiceCode.RangedMissingTransform,
                    $"Entity {entity.ID} missing TransformInstance for using ranged.");

            var spawnContext = new ProjectileEntityCreateContext(
                Guid.NewGuid().ToString(),
                config.EntityDefinitionID,
                transform.RoomSpatialID,
                transform.LayerZ,
                transform.Position,
                targetVector
            );

            entitySpawnService.Spawn(spawnContext);
        }

        private void ExecuteMelee(
            EntityInstance entity,
            MeleeConfig config,
            Vector2 targetVector)
        {
            var transform = entity.GetComponent<TransformInstance>();
            if (transform == null)
                throw new InternalException(
                    ApplicationCode.ItemServiceCode.MeleeMissingTransform,
                    $"Entity {entity.ID} missing TransformInstance for using melee.");

            var spawnContext = new WorldEntityCreateContext(
                Guid.NewGuid().ToString(),
                config.EntityDefinitionID,
                transform.RoomSpatialID,
                transform.LayerZ,
                targetVector
            );

            entitySpawnService.Spawn(spawnContext);
        }

        private void ExecuteConsumable(
            EntityInstance entity,
            ConsumableConfig config)
        {
            config.EffectDefinitionIDs.ForEach(e => effectService.ApplyEffect(entity, e));
        }
        #endregion

        #region Private Cost Steps (Inventory Mutations)
        private void DeductCost(
            EntityInstance entity,
            ItemInstance item,
            ItemDefinition itemDef)
        {
            if (itemDef.CostConfig == null || itemDef.CostConfig.Method == ItemConsumptionMethod.None)
            {
                return;
            }

            switch (itemDef.CostConfig.Method)
            {
                case ItemConsumptionMethod.ConsumeStack:
                    inventoryService.DeductItem(entity, item);
                    break;

                case ItemConsumptionMethod.DegradeDurability:
                    inventoryService.DegradeItem(entity, item);
                    break;

                case ItemConsumptionMethod.RemoveEntirely:
                    inventoryService.RemoveItem(entity, item);
                    break;
            }
        }
        #endregion
    }
}