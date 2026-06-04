using Contract.Enum.EntityDomain;
using Domain.Abstraction;
using Domain.Common;
using Domain.Runtime.AttributeDomain;
using Domain.Runtime.EntityDomain.Component;
using Domain.Runtime.ItemDomain;

namespace Domain.Runtime.EntityDomain
{
    public class CreatureInstance : EntityInstance
    {
        #region Attributes
        private readonly Dictionary<EquipmentSlot, ItemInstance?> equipment;
        #endregion

        #region Properties
        public CharacteristicInstance Characteristic { get; private set; }
        public InventoryInstance Inventory { get; private set; }
        public int Level { get; private set; }
        public List<EffectInstance> ActiveEffects { get; private set; }
        #endregion

        public CreatureInstance(
            string id,
            string definitionId,
            ICollisionShape collisionShape,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            Vector2 direction,
            AppearanceInstance appearance,
            CharacteristicInstance characteristic,
            InventoryInstance inventory,
            int level,
            List<EffectInstance> activeEffects) : base(
                id, 
                definitionId,
                collisionShape,
                roomSpatialId,
                layerZ,
                position, 
                direction,
                appearance)
        {
            equipment = new();
            Characteristic = characteristic;
            Inventory = inventory;
            Level = level;
            ActiveEffects = activeEffects;
        }

        #region Methods
        public IReadOnlyDictionary<EquipmentSlot, ItemInstance?> GetEquipment()
        {
            return equipment;
        }

        public ItemInstance? GetEquipment(
            EquipmentSlot slot)
        {
            equipment.TryGetValue(slot, out var item);
            return item;
        }

        public void SetEquipment(
            EquipmentSlot slot,
            ItemInstance? item)
        {
            equipment[slot] = item;
        }

        public void RemoveEquipment(
            EquipmentSlot slot)
        {
            equipment[slot] = null;
        }
        #endregion
    }
}