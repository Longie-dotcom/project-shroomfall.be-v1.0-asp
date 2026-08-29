using Contract.DTO.Definition.WorldDomain;

namespace Application.Interface.Cache.WorldDomain
{
    public interface ICombatRunCache
    {
        void Load(
            IEnumerable<CombatRunDefinitionDTO> data);
        IReadOnlyCollection<CombatRunDefinitionDTO> GetAll();
        CombatRunDefinitionDTO? Get(
            string id);
    }
}
