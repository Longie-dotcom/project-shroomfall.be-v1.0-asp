using Domain.Abstraction;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Snapshot.EntityDomain
{
    public class EntitySnapshot : ISnapshot
    {
        [BsonId]
        public string ID { get; set; } = string.Empty;
        public string DefinitionID { get; set; } = string.Empty;
        public List<ComponentSnapshot> Components { get; set; } = new();

        public T? GetComponent<T>() where T : ComponentSnapshot
        {
            return Components.OfType<T>().FirstOrDefault();
        }
    }
}