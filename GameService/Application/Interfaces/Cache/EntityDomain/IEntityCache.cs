using Domain.Definition.EntityDomain;

namespace Application.Interfaces.Cache.EntityDomain
{
    public interface IEntityCache
    {
        void Load(
            List<EntityDefinition> data);
        IEnumerable<EntityDefinition> GetAll();
        EntityDefinition? Get(
            string id);
    }
}
