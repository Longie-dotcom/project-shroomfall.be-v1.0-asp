using Domain.Common;
using Domain.Definition.WorldDomain;
using Domain.Definition.WorldDomain.Enum;

namespace Application.Services.Abstraction.WorldService
{
    public interface ISpawnService
    {
        (Vector2 position, int layerZ) ResolveSpawnPosition(
            string roomDefinitionId,
            SpawnArea area);
        public (Vector2 position, int layerZ) ResolveSpawnPosition(
            string roomDefinitionId,
            string entityDefinitionId,
            SpawnRuleType type);
        SpawnArea PickWeightedArea(
            ICollection<SpawnArea> areas);
    }
}
