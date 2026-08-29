using Contract.DTO.Definition.MetaDomain;

namespace Application.Interface.Cache.MetaDomain
{
    public interface IItemCache
    {
        void Load(
            List<ItemDefinitionDTO> data);
        IEnumerable<ItemDefinitionDTO> GetAll();
        ItemDefinitionDTO? Get(
            string id);
    }
}
