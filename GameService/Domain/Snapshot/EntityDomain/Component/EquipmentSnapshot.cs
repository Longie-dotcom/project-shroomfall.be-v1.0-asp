using Contract.Enum.MetaDomain.Item;
using Domain.Abstraction;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Domain.Snapshot.EntityDomain.Component
{
    public class EquipmentSnapshot : ComponentSnapshot
    {
        [BsonDictionaryOptions(DictionaryRepresentation.Document)]
        public Dictionary<EquipmentSlot, ItemSnapshot?> Slots { get; set; } = new();
    }
}