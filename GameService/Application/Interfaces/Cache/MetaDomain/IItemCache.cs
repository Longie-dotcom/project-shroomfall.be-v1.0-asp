using Domain.Definition.MetaDomain;

namespace Application.Interfaces.Cache.MetaDomain
{
    public interface IItemCache
    {
        void Load(
            List<ItemDefinition> data);
        IEnumerable<ItemDefinition> GetAll();
        ItemDefinition? Get(
            string id);
    }
}
