using Domain.Definition.AttributeDomain;
using Domain.Definition.AttributeDomain.Enum;

namespace Application.Interfaces.Cache
{
    public interface IAttributeValueCache
    {
        void Load(
            IEnumerable<AttributeValue> data);
        IReadOnlyCollection<AttributeValue> GetAll();
        AttributeValue? Get(
            string id, 
            AttributeType type, 
            int level);
    }
}
