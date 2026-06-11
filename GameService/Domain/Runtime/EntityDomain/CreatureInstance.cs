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
        private readonly Dictionary<string, float> threatTable;
        #endregion

        #region Properties
        public CharacteristicInstance Characteristic { get; private set; }
        public InventoryInstance Inventory { get; private set; }
        public int Level { get; private set; }
        public List<EffectInstance> ActiveEffects { get; private set; }

        // Runtime only
        public float AttackTimer { get; set; }
        public bool IsAIControlled { get; set; }
        public AIState AIState { get; set; }
        public Vector2 HomePosition { get; private set; }
        public string? TargetEntityId { get; set; }
        public IReadOnlyDictionary<string, float> ThreatTable
            => threatTable;
        public float LeashDistance { get; set; }
        public float ThinkCooldownRemaining { get; set; }
        #endregion

        public CreatureInstance(
            string id,
            string definitionId,
            ICollisionShape collisionShape,
            Vector2 collisionOffset,
            string roomSpatialId,
            int layerZ,
            Vector2 position,
            Vector2 movementVector,
            AppearanceInstance appearance,
            CharacteristicInstance characteristic,
            InventoryInstance inventory,
            int level,
            List<EffectInstance> activeEffects) : base(
                id, 
                definitionId,
                collisionShape,
                collisionOffset,
                roomSpatialId,
                layerZ,
                position,
                movementVector,
                appearance)
        {
            equipment = new();
            threatTable = new();

            Characteristic = characteristic;
            Inventory = inventory;
            Level = level;
            ActiveEffects = activeEffects;
            
            AttackTimer = 0f;
            IsAIControlled = true;
            AIState = AIState.Idle;
            HomePosition = position;
            ThinkCooldownRemaining = 0f;
            TargetEntityId = null;
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

        public void AddThreat(
            string entityId,
            float amount)
        {
            threatTable.TryGetValue(entityId, out float current);
            threatTable[entityId] = current + amount;
        }

        public void ClearThreat()
        {
            threatTable.Clear();
        }
        #endregion
    }
}