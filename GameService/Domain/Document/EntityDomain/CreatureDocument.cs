using Domain.Document.AttributeDomain;
using Domain.Document.ItemDomain;
using Domain.Runtime.EntityDomain.Enum;
using Domain.Runtime.ItemDomain;

namespace Domain.Document.EntityDomain
{
    public class CreatureDocument : EntityDocument
    {
        public CharacteristicDocument Characteristic { get; set; } = new();
        public InventoryDocument Inventory { get; set; } = new();
        public int Level { get; set; }
        public List<EffectDocument> ActiveEffects { get; set; } = new();
        public Dictionary<EquipmentSlot, ItemInstance?> Equipment { get; } = new();
    }
}