using Application.Events.Event;
using Application.Interfaces.Cache;
using Application.Interfaces.Realtime;
using Contract.Enum.EntityDomain;
using Contract.Enum.ItemDomain;
using Domain.Common;
using Domain.DomainException;
using Domain.Runtime.EntityDomain;
using Domain.Shared;

namespace Application.Services.ItemService
{
    public class ItemService
    {
        #region Attributes
        private readonly IEventBus eventBus;
        private readonly IItemCache itemCache;
        private readonly ItemUsageService itemUsageService;
        #endregion

        #region Properties
        #endregion

        public ItemService(
            IEventBus eventBus,
            IItemCache itemCache,
            ItemUsageService itemUsageService)
        {
            this.eventBus = eventBus;
            this.itemCache = itemCache;
            this.itemUsageService = itemUsageService;
        }

        #region Methods
        public void Use(
            CreatureInstance creature,
            string itemInstanceId,
            Vector2 targetPosition)
        {
            var inventory = creature.Inventory;
            var item = inventory.Items.FirstOrDefault(x => x.ID == itemInstanceId);
            if (item == null)
                throw new BadRequest(ResponseCode.ItemService_ItemNotFoundInInventory);

            var itemDef = itemCache.Get(item.DefinitionID);
            if (itemDef == null)
                throw new InternalException(ResponseCode.ItemService_ItemDefinitionNotFound);

            // Execute the centralized usage engine
            itemUsageService.Execute(creature, item, itemDef, targetPosition);

            eventBus.Publish(new EntityActedEvent(
                creature.ID,
                creature.RoomSpatialID,
                creature.Position,
                creature.FacingDirection,
                itemDef.DefaultAction == EntityAction.NONE ? creature.CurrentAction : itemDef.DefaultAction,
                item.DefinitionID
            ));
        }

        public void Unequip(
            CreatureInstance creature,
            EquipmentSlot slot)
        {
            itemUsageService.Unequip(creature, slot);
        }
        #endregion
    }
}