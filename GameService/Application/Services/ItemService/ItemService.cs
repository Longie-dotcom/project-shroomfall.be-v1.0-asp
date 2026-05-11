using Application.Interfaces.Cache;
using Application.Services.Abstraction.ItemService;
using Domain.Common;
using Domain.Definition.ItemDomain.Enum;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Shared;

namespace Application.Services.ItemService
{
    public class ItemService : IItemService
    {
        #region Attributes
        private readonly IItemCache itemCache;
        private readonly IEquipmentService equipmentService;
        private readonly IConsumableService consumableService;
        private readonly IPlacementService placementService;
        #endregion

        #region Properties
        #endregion

        public ItemService(
            IItemCache itemCache,
            IEquipmentService equipmentService,
            IConsumableService consumableService, 
            IPlacementService placementService)
        {
            this.itemCache = itemCache;
            this.equipmentService = equipmentService;
            this.consumableService = consumableService;
            this.placementService = placementService;
        }

        #region Methods
        public void Use(
            CreatureInstance creature, 
            string itemInstanceId,
            Vector2 objectPosition)
        {
            var inventory = creature.Inventory;

            // Find item from inventory
            var item = inventory.Items.FirstOrDefault(x => x.ID == itemInstanceId);
            if (item == null)
                throw new BadRequest(ResponseCode.ItemService_ItemNotFound);

            // Find item definition from cache
            var itemDef = itemCache.Get(item.DefinitionID);
            if (itemDef == null)
                throw new InternalException(ResponseCode.ItemService_ItemDefinitionNotFound);

            switch (itemDef.Type)
            {
                case ItemType.Equippable:
                    equipmentService.Equip(creature, item, itemDef);
                    break;

                case ItemType.Consumable:
                    consumableService.Consume(creature, item, itemDef);
                    break;

                case ItemType.Object:
                    placementService.Place(creature, item, itemDef, objectPosition);
                    break;

                default:
                    throw new BadRequest(ResponseCode.ItemService_TypeNotSupported);
            }
        }
        #endregion
    }
}