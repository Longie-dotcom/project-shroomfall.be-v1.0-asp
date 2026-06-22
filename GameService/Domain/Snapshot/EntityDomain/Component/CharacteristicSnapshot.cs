using Contract.Enum.MetaDomain.Effect;
using Domain.Abstraction;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Domain.Snapshot.EntityDomain.Component
{
    public class CharacteristicSnapshot : ComponentSnapshot
    {
        public int CurrentLevel { get; set; }

        [BsonDictionaryOptions(DictionaryRepresentation.Document)]
        public Dictionary<AttributeType, float> Vitals { get; set; } = new();

        [BsonDictionaryOptions(DictionaryRepresentation.Document)]
        public Dictionary<AttributeType, float> Cores { get; set; } = new();
    }
}