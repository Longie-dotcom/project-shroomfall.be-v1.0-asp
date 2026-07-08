using Application.Interfaces.Cache;
using Application.Services.AttributeService;
using Application.Services.WorldService;
using Contract.Enum.MetaDomain.Item;
using Domain.Common;
using Domain.Definition.MetaDomain;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.EntityDomain.Component;
using ResponseCode;

namespace Application.Services.ItemService
{
    public class ItemUsageService
    {
        #region Attributes
        private readonly ICacheProvider cacheProvider;
        private readonly EntitySpawnService entitySpawnService;
        private readonly EffectService effectService;
        private readonly InventoryService inventoryService;
        #endregion

        public ItemUsageService(
            ICacheProvider cacheProvider,
            EntitySpawnService entitySpawnService,
            EffectService effectService,
            InventoryService inventoryService)
        {
            this.cacheProvider = cacheProvider;
            this.entitySpawnService = entitySpawnService;
            this.effectService = effectService;
            this.inventoryService = inventoryService;
        }

        #region Core Pipeline
        /// <summary>
        /// Orchestrates the decoupled Manifestation and Cost logic.
        /// </summary>
        public void Execute(
            EntityInstance entity,
            ItemInstance item,
            ItemDefinition itemDef,
            Vector2 targetVector)
        {
            // 1. Manifest the item's unique identity in the gameplay world (Spawn, Equip, Buff)
            ExecuteManifestation(entity, item, itemDef, targetVector);

            // 2. Charge the asset cost safely (Consume, Degrade, Transfer)
            DeductCost(entity, item, itemDef);
        }
        #endregion

        #region Private Manifestation Steps (World Changes)
        private void ExecuteManifestation(
            EntityInstance entity,
            ItemInstance item,
            ItemDefinition itemDef,
            Vector2 targetVector)
        {
            // ─────────────────────────────
            // 1. Spawning Logic (Projectiles, AoE, Placeables)
            // ─────────────────────────────
            if (itemDef.SpawnEntityConfig != null)
            {
                var transform = entity.GetComponent<TransformInstance>();
                if (transform == null)
                    throw new InternalException(
                        ApplicationCode.ItemUsageServiceCode.ExecuteEntityMissingTransform,
                        $"Entity {entity.ID} missing TransformInstance.");

                WorldEntityCreateContext spawnContext;
                var config = itemDef.SpawnEntityConfig;
                var instanceId = Guid.NewGuid().ToString();

                switch (config.TargetType)
                {
                    case SpawnTargetType.Directional:
                        spawnContext = new ProjectileEntityCreateContext(
                            instanceId,
                            config.EntityDefinitionID,
                            transform.RoomSpatialID,
                            transform.LayerZ,
                            transform.Position, // Origin point
                            targetVector        // The direction/velocity vector
                        );
                        break;

                    case SpawnTargetType.AoE:
                        Vector2 finalSpawnPosition = targetVector;

                        // Apply Range Constraint (Clamping)
                        if (config.MaxRange > 0)
                        {
                            float dist = Vector2.Distance(transform.Position, targetVector);
                            if (dist > config.MaxRange)
                            {
                                // Normalize the direction and multiply by range
                                Vector2 direction = Vector2.Normalize(targetVector - transform.Position);
                                finalSpawnPosition = transform.Position + (direction * config.MaxRange);
                            }
                        }

                        spawnContext = new WorldEntityCreateContext(
                            instanceId,
                            config.EntityDefinitionID,
                            transform.RoomSpatialID,
                            transform.LayerZ,
                            finalSpawnPosition
                        );
                        break;

                    case SpawnTargetType.WorldPosition:

                    default:
                        spawnContext = new WorldEntityCreateContext(
                            instanceId,
                            config.EntityDefinitionID,
                            transform.RoomSpatialID,
                            transform.LayerZ,
                            targetVector        // The target destination
                        );
                        break;
                }

                entitySpawnService.Spawn(spawnContext);
            }

            // ─────────────────────────────
            // 2. Effect Application (Consumables, Buffs, Healing)
            // ─────────────────────────────
            if (itemDef.ApplyEffectConfig != null)
            {
                itemDef.ApplyEffectConfig.EffectDefinitionIDs
                    .ForEach(e => effectService.ApplyEffect(entity, e));
            }

            // ─────────────────────────────
            // 3. Equipment Configuration (Armor, Weapons)
            // ─────────────────────────────
            if (itemDef.EquipConfig != null)
            {
                PerformEquip(entity, item, itemDef);
            }
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
                    for (int i = 0; i < itemDef.CostConfig.Value; i++)
                    {
                        inventoryService.DeductItem(entity, item);
                    }
                    break;

                case ItemConsumptionMethod.DegradeDurability:
                    for (int i = 0; i < itemDef.CostConfig.Value; i++)
                    {
                        inventoryService.DegradeItem(entity, item, itemDef.CostConfig.Value);
                    }
                    break;

                case ItemConsumptionMethod.RemoveEntirely:
                    inventoryService.RemoveItem(entity, item);
                    break;
            }
        }
        #endregion

        #region Equipment Operations
        private void PerformEquip(
            EntityInstance entity,
            ItemInstance item,
            ItemDefinition itemDef)
        {
            if (item.Amount != 1)
                throw new InternalException(
                    ApplicationCode.ItemUsageServiceCode.EquipInvalidItem,
                    $"Cannot equip item stack. Amount must be exactly 1. Current amount: {item.Amount}");

            var equipment = entity.GetComponent<EquipmentInstance>();
            if (equipment == null)
                throw new InternalException(
                    ApplicationCode.ItemUsageServiceCode.EquipEquipmentMissing,
                    $"Entity {entity.ID} missing EquipmentInstance.");

            var slot = itemDef.EquipConfig!.Slot;

            if (equipment.Slots[slot] != null)
                throw new InternalException(
                    ApplicationCode.ItemUsageServiceCode.EquipSlotOccupied,
                    $"Cannot equip item. Slot {slot} is already occupied.");

            equipment.Equip(slot, item);
        }

        public void Unequip(
            EntityInstance entity,
            EquipmentSlot slot)
        {
            var equipment = entity.GetComponent<EquipmentInstance>();
            if (equipment == null) return;

            var equippedItem = equipment.Slots[slot];
            if (equippedItem == null) return;

            var itemDef = cacheProvider.Item.Get(equippedItem.DefinitionID);
            if (itemDef == null)
                throw new InternalException(
                    ApplicationCode.ItemUsageServiceCode.UnequipItemDefinitionNotFound,
                    $"Item definition not found in cache: {equippedItem.DefinitionID}");

            if (!inventoryService.CanAddItem(entity, equippedItem))
                throw new InternalException(
                    ApplicationCode.ItemUsageServiceCode.UnequipInventoryFull,
                    "Cannot unequip item. Not enough space in inventory.");

            equipment.Unequip(slot);

            if (itemDef.ApplyEffectConfig != null)
            {
                itemDef.ApplyEffectConfig.EffectDefinitionIDs
                    .ForEach(e => effectService.RemoveEffect(entity, e));
            }

            var remainder = inventoryService.AddItem(entity, equippedItem);

            if (remainder != null)
            {
                equipment.Equip(slot, equippedItem);

                if (itemDef.ApplyEffectConfig != null)
                    itemDef.ApplyEffectConfig.EffectDefinitionIDs
                        .ForEach(e => effectService.ApplyEffect(entity, e));

                throw new InternalException(
                    ApplicationCode.ItemUsageServiceCode.UnequipTransactionFailed,
                    "Unequip transaction failed. Inventory capacity changed during operation.");
            }
        }
        #endregion
    }
}