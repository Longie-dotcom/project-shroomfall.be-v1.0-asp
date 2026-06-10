using Application.Coordinator;
using Application.Interfaces.Cache;
using Application.Services.AttributeService;
using Contract.Enum.AttributeDomain;
using Contract.Enum.EntityDomain;
using Contract.Enum.ItemDomain;
using Domain.Common;
using Domain.Definition.EntityDomain;
using Domain.Definition.ItemDomain;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Runtime.ItemDomain;
using Domain.Shared;

namespace Application.Services.ItemService
{
    public class ItemUsageService
    {
        #region Attributes
        private readonly EntityLifeCycleCoordinator entityLifeCycleCoordinator;
        private readonly EffectService effectService;
        private readonly InventoryService inventoryService;
        private readonly IItemCache itemCache;
        #endregion

        public ItemUsageService(
            EntityLifeCycleCoordinator entityLifeCycleCoordinator,
            EffectService effectService,
            InventoryService inventoryService,
            IItemCache itemCache)
        {
            this.entityLifeCycleCoordinator = entityLifeCycleCoordinator;
            this.effectService = effectService;
            this.inventoryService = inventoryService;
            this.itemCache = itemCache;
        }

        #region Core Pipeline
        /// <summary>
        /// Orchestrates the decoupled Manifestation and Cost logic.
        /// </summary>
        public void Execute(CreatureInstance creature, ItemInstance item, Item itemDef, Vector2 targetVector)
        {
            // Manifest the item's unique identity in the gameplay world
            ExecuteManifestation(creature, item, itemDef, targetVector);

            // Charge the asset cost safely
            DeductCost(creature, item, itemDef);
        }
        #endregion

        #region Private Manifestation Steps (World Changes)
        private void ExecuteManifestation(
            CreatureInstance creature, 
            ItemInstance item, 
            Item itemDef, 
            Vector2 targetVector)
        {
            switch (itemDef.Type)
            {
                case ItemType.RangedWeapon:
                case ItemType.ThrowableWeapon:
                    ProjectileWeapon(targetVector, creature, itemDef, item);
                    break;

                case ItemType.MeleeWeapon:
                    MeleeWeapon(targetVector, creature, itemDef, item);
                    break;

                case ItemType.Placeable:
                    entityLifeCycleCoordinator.SpawnWorldObject(
                        worldObjectDefinitionId: item.DefinitionID,
                        roomSpatialId: creature.RoomSpatialID,
                        layerZ: creature.LayerZ,
                        position: targetVector,
                        direction: Vector2.Zero
                    );
                    break;

                case ItemType.Consumable:
                    effectService.ApplyItemEffects(creature, itemDef, item.ID);
                    break;

                case ItemType.Equippable:
                    PerformEquip(creature, item, itemDef);
                    break;
            }
        }
        #endregion

        #region Private Cost Steps (Inventory Mutations)
        private void DeductCost(
            CreatureInstance creature,
            ItemInstance item,
            Item itemDef)
        {
            switch (itemDef.Type)
            {
                case ItemType.ThrowableWeapon:
                case ItemType.Consumable:
                case ItemType.Placeable:
                    inventoryService.DeductItem(creature, item);
                    break;

                case ItemType.RangedWeapon:
                case ItemType.MeleeWeapon:
                    inventoryService.DegradeItem(creature, item);
                    break;

                case ItemType.Equippable:
                    inventoryService.RemoveItem(creature, item);
                    break;
            }
        }
        #endregion

        #region Weapon Operations
        public void ProjectileWeapon(
            Vector2 targetVector,
            CreatureInstance creature,
            Item itemDef,
            ItemInstance item)
        {
            var direction = Vector2.Normalize(
                targetVector - creature.Position
            );

            entityLifeCycleCoordinator.SpawnProjectile(
                projectileDefinitionId: item.DefinitionID,
                roomSpatialId: creature.RoomSpatialID,
                layerZ: creature.LayerZ,
                position: creature.Position,
                direction: direction,
                ownerId: creature.ID,
                sourceDefinitionId: itemDef.ID
            );
        }

        public void MeleeWeapon(
            Vector2 targetVector,
            CreatureInstance creature,
            Item itemDef,
            ItemInstance item)
        {
            Vector2 toTarget = targetVector - creature.Position;

            float distance = toTarget.Length();
            float range = creature.Characteristic.GetCore(AttributeType.AttackRange);

            Vector2 castPosition;

            if (distance > range)
            {
                Vector2 direction = Vector2.Normalize(toTarget);
                castPosition = creature.Position + direction * range;
            }
            else
            {
                castPosition = targetVector;
            }

            entityLifeCycleCoordinator.SpawnAreaEffect(
                areaEffectDefinitionId: item.DefinitionID,
                roomSpatialId: creature.RoomSpatialID,
                layerZ: creature.LayerZ,
                position: creature.Position,
                ownerId: creature.ID,
                sourceDefinitionId: itemDef.ID
            );
        }
        #endregion

        #region Equipment Operations
        private void PerformEquip(
            CreatureInstance creature,
            ItemInstance item,
            Item itemDef)
        {
            if (item.Count != 1)
                throw new BadRequest(
                    ResponseCode.EquipmentService_InvalidItem,
                    $"Cannot equip item stack. Count must be exactly 1. Current count: {item.Count}");

            if (!EquipmentMapping.Map.TryGetValue(itemDef.Category, out var slot))
                throw new BadRequest(ResponseCode.EquipmentService_InvalidItem);

            if (creature.GetEquipment(slot) != null)
                throw new BadRequest(ResponseCode.EquipmentService_EquipmentSlotOccupied);

            creature.SetEquipment(slot, item);
            effectService.ApplyItemEffects(creature, itemDef, item.ID);
        }

        public void Unequip(
            CreatureInstance creature,
            EquipmentSlot slot)
        {
            var equipped = creature.GetEquipment(slot);
            if (equipped == null) return;

            if (!inventoryService.CanAddItem(creature, equipped))
                throw new BadRequest(ResponseCode.EquipmentService_InventoryFullOnUnequip);

            var itemDef = itemCache.Get(equipped.DefinitionID);
            if (itemDef == null)
                throw new InternalException(ResponseCode.EquipmentService_ItemDefinitionNotFound);

            creature.RemoveEquipment(slot);
            effectService.RemoveItemEffects(creature, equipped.ID);

            var remainder = inventoryService.AddItem(creature, equipped);
            if (remainder != null)
            {
                // Transactional Recovery fallback
                creature.SetEquipment(slot, equipped);
                effectService.ApplyItemEffects(creature, itemDef, equipped.ID);
                throw new InternalException(ResponseCode.EquipmentService_InventoryFullOnUnequip);
            }
        }
        #endregion
    }
}