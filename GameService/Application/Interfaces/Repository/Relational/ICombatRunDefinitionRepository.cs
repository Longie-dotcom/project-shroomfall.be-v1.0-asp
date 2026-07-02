using Application.Interfaces.Repository.Base;
using Domain.Definition.WorldDomain;

namespace Application.Interfaces.Repository.Relational
{
    public interface ICombatRunDefinitionRepository : ISQLGenericRepository<CombatRunDefinition>, IRelationalRepository
    {
        Task UpsertFloorsAsync(
            string combatRunDefinitionId,
            IEnumerable<Floor> floors);
    }
}
