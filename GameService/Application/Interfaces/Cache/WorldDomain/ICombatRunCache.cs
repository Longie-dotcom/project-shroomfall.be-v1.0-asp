using Domain.Definition.WorldDomain;

namespace Application.Interfaces.Cache.WorldDomain
{
    public interface ICombatRunCache
    {
        void Load(
            IEnumerable<CombatRunDefinition> data);
        IReadOnlyCollection<CombatRunDefinition> GetAll();
        CombatRunDefinition? Get(
            string id);
    }
}
