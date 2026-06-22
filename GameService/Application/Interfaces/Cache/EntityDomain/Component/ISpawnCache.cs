using Domain.Definition.EntityDomain.Component;

namespace Application.Interfaces.Cache.EntityDomain.Component
{
    public interface ISpawnCache
    {
        void Load(
            List<SpawnDefinition> data);
        IEnumerable<SpawnDefinition> GetAll();
        SpawnDefinition? Get(
            Guid id);
        SpawnDefinition? GetByEntity(
            string entityDefinitionId);
    }
}
